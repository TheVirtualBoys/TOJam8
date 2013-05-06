using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    public class PhysicsSprite : Sprite
    {
        private const int width = 29;
        private const int height = 34;

        Vector2 m_position;
        Vector2 m_velocity; //px / s
        Vector2 m_acceleration; //px / s / s
        double m_lastDelta;
        double m_jumpAccelTime;
        bool m_jumping;
        int lastAniIndex;
        double m_dyingTimer;
        const double m_deathTime = 2.5;

		KeyboardState oldKeyboard;

		public enum PlayerType
		{
			Player1,
			Player2
		}
		
		PlayerType playerType;

		public enum GlowType
		{
			Red_Glow,
			Blue_Glow
		}

		GlowType glowType;

		public const int ANI_IDLE = 0;
		public const int ANI_RUN = 1;
		public const int ANI_JUMP_LEAP = 2;
		public const int ANI_JUMP_HANG = 3;
		public const int ANI_JUMP_DROP = 4;
		public const int ANI_DIE = 5;

		public const int ANI_P1 = 0;
		public const int ANI_P2 = 6;
		public const int ANI_RED_GLOW = 17;
		public const int ANI_BLUE_GLOW = 22;

        public PhysicsSprite(AnimationData aniData, AnimationData glowAniData, PlayerType playerType)
            : base( aniData, glowAniData)
            {
				this.playerType = playerType;
                m_position = (playerType == PlayerType.Player1)? new Vector2(130, 120) : new Vector2(32, 120);
                m_velocity = m_acceleration = new Vector2(0, 0);
                m_lastDelta = m_jumpAccelTime = 0.0;
                m_jumping = true; //flying start
                lastAniIndex = -1; //set impossible state for refresh
				glowType = PhysicsSprite.GlowType.Red_Glow;
                m_dyingTimer = 0.0;
            }

		public PlayerType PlayerId
		{
			get { return playerType; }
		}

		public GlowType PlayerGlowType
		{
			get { return glowType; }
			set { glowType = value; }
		}

        public Vector2 Acceleration
        {
            get { return m_acceleration; }
            set { this.m_acceleration = value; }
        }

        public Vector2 Velocity
        {
            get { return m_velocity; }
            set { this.m_velocity = value; }
        }

		private int getPlayerAniIndex(int aniTypeIndex)
		{
			return ((PlayerId == PlayerType.Player1) ? ANI_P1 : ANI_P2) + aniTypeIndex;
		}

		private int getPlayerGlowAniIndex(int glowAniTypeIndex)
		{
			int index = -1;
			if (glowAniTypeIndex != ANI_DIE)
				index = ((PlayerGlowType == GlowType.Red_Glow) ? ANI_RED_GLOW : ANI_BLUE_GLOW) + glowAniTypeIndex;
			return index;
		}

        public TileSet.Bounds Collides(Vector2 start, Vector2 end, out Vector2 hit)
        {
            int hit_x = 0, hit_y = 0;
            TileSet.Bounds bounds = Game1.Instance.ray((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, out hit_x, out hit_y);
            hit = new Vector2(hit_x, hit_y);
            return bounds; 
        }

        public bool CollisionDownHelper( bool collides, Vector2 offset, Vector2 position, double y, out double outY )
        {
            Vector2 hitPos;
            Collides( m_position + offset, position + offset + new Vector2(1, 1), out hitPos );
            bool localCollides = hitPos.X != -1 && hitPos.Y != -1;
            if (localCollides)
            {
                outY = Math.Min(y, hitPos.Y);
                collides |= localCollides;
            }
            else outY = y;
            return collides;
        }

        public bool CollisionUpHelper(bool collides, Vector2 offset, Vector2 position, double y, out double outY)
        {
            Vector2 hitPos;
            Collides(m_position + offset, position + offset, out hitPos);
            bool localCollides = hitPos.X != -1 && hitPos.Y != -1;
            if (localCollides)
            {
                outY = Math.Max(y, hitPos.Y);
                collides |= localCollides;
            }
            else outY = y;
            return collides;
        }

        public bool CollisionRightHelper(bool pastCollides, Vector2 offset, Vector2 position, double y, out double outY, bool isSlash, out bool outIsSlash, bool isBSlash, out bool outIsBSlash)
        {
            Vector2 hitPos;
            TileSet.Bounds bound = Collides(m_position + offset, position + offset + new Vector2(1, 0), out hitPos);

            outIsSlash = isSlash | (bound & TileSet.Bounds.BOUNDS_SLASH) == TileSet.Bounds.BOUNDS_SLASH;
            outIsBSlash = isBSlash | (bound & TileSet.Bounds.BOUNDS_BSLASH) == TileSet.Bounds.BOUNDS_BSLASH;
            bool collided = hitPos.X != -1 && hitPos.Y != -1;
            outY = y;
            if (collided)
            {
                outY = Math.Min(y, hitPos.Y);
            }
            return pastCollides | collided;
        }

		public void toggleGlowType()
		{
			PlayerGlowType = (PlayerGlowType == GlowType.Red_Glow) ? GlowType.Blue_Glow : GlowType.Red_Glow;
		}

        public override void update(GameTime frameTime)
        {
            base.update(frameTime);

            float dt = (float)(frameTime.ElapsedGameTime.Seconds + frameTime.ElapsedGameTime.Milliseconds * 0.001);

            int newAniIndex = ANI_IDLE;
            if (m_dyingTimer > 0)
            {
                newAniIndex = ANI_DIE;
                m_dyingTimer -= dt;
                byte lum = (byte)(255 * m_dyingTimer / m_deathTime);
                m_color.R = m_color.G = m_color.B = lum;
                if ( m_dyingTimer <=  0.0 )
                {
                    m_dyingTimer = 0.0;
                    Game1.Instance.setState(Game1.State.STATE_SCORES);
                }
            }
            else
            {
                m_velocity += m_acceleration * dt;
                Vector2 position = m_position + m_velocity * dt;
                m_lastDelta = dt;

                bool killVeloX = false;
                bool killVeloY = false;
                if (position.X < 0)
                {
                    killVeloX = true;
                    position.X = 0;
                }
                if (position.X + width >= 256)
                {
                    killVeloX = true;
                    position.X = 255 - width;
                }
                if (position.Y < 0)
                {
                    killVeloY = true;
                    position.Y = 0;
                }
                if (position.Y + height >= 240)
                {
                    killVeloY = true;
                    toggleGlowType();
                    position.Y = 239 - height;
                }

                //downward checks (left middle right)
                {
                    double minY = 240;
                    bool collide = false;
                    float insideHeight = height - 1;
                    Vector2 insideLeftBotOffset = new Vector2(0, insideHeight);
                    Vector2 insideMidBotOffset = new Vector2(width / 2, insideHeight);
                    Vector2 insideRightBotOffset = new Vector2(width - 1, insideHeight);
                    //                collide = CollisionDownHelper(collide, insideLeftBotOffset, position, minY, out minY);
                    collide = CollisionDownHelper(collide, insideMidBotOffset, position, minY, out minY);
                    collide = CollisionDownHelper(collide, insideRightBotOffset, position, minY, out minY);
                    minY -= insideHeight + 1;
                    if (collide)
                    {
                        killVeloY = true;
                        position.Y = Math.Max(0, (float)minY); toggleGlowType();
                    }
                }
                //up checks (left, middle, right)
                {
                    double maxY = 0;
                    bool collide = false;
                    Vector2 insideLeftTopOffset = new Vector2(0, 0);
                    Vector2 insideMidTopOffset = new Vector2(width / 2, 0);
                    Vector2 insideRightTopOffset = new Vector2(width - 1, 0);
                    collide = CollisionUpHelper(collide, insideLeftTopOffset, position, maxY, out maxY);
                    collide = CollisionUpHelper(collide, insideMidTopOffset, position, maxY, out maxY);
                    collide = CollisionUpHelper(collide, insideRightTopOffset, position, maxY, out maxY);
                    if (collide)
                    {
                        killVeloY = true;
                        position.Y = Math.Min(239 - height, (float)maxY + 1);
                    }
                }

                //fwd checks (in quarters top to bottom)
                {
                    bool collide = false, isSlash = false, isBSlash = false;
                    float insideWidth = width - 1;
                    double minY = 240;
                    Vector2 insideRight0Offset = new Vector2(insideWidth, 0);
                    Vector2 insideRight1Offset = new Vector2(insideWidth, height / 3);
                    Vector2 insideRight2Offset = new Vector2(insideWidth, 2 * height / 3);
                    Vector2 insideRight3Offset = new Vector2(insideWidth, height - 1);
                    double fakeY = 240;
                    collide = CollisionRightHelper(collide, insideRight0Offset, position, fakeY, out fakeY, isSlash, out isSlash, isBSlash, out isBSlash);
                    collide = CollisionRightHelper(collide, insideRight1Offset, position, fakeY, out fakeY, isSlash, out isSlash, isBSlash, out isBSlash);
                    collide = CollisionRightHelper(collide, insideRight2Offset, position, minY, out minY, isSlash, out isSlash, isBSlash, out isBSlash);
                    collide = CollisionRightHelper(collide, insideRight3Offset, position, minY, out minY, isSlash, out isSlash, isBSlash, out isBSlash);
                    if (collide)
                    {
                        minY -= height; //wrong for all but last who cares
                        if (isSlash || isBSlash) //slide
                        {
                            position.Y = Math.Min(239 - height, (float)minY - 3);
                        }
                        else
                        {
                            killVeloX = true;
                            //m_color = Color.Red;
                            m_dyingTimer = m_deathTime;
                            foreach (Layer layer in Game1.Instance.gameData.layers)
                            {
                                layer.setSpeed(0.0);
                            }
                        }
                    }
                    /*else
                    {
                        m_color = Color.White;
                    }
                    */
                }


                if (killVeloX) m_velocity.X = 0;
                if (killVeloY) m_velocity.Y = 0;

                m_position = position;

                //patch back the info
                Left = (int)m_position.X;
                Top = (int)m_position.Y;

                if (m_velocity.Y != 0.0)
                {
                    double threshold = 120;
                    if (m_velocity.Y < -threshold) newAniIndex = ANI_JUMP_LEAP;
                    else if (m_velocity.Y >= -threshold && m_velocity.Y < threshold)
                    {
                        newAniIndex = ANI_JUMP_HANG;
                    }
                    else newAniIndex = ANI_JUMP_DROP;
                }
                else
                {
                    newAniIndex = ANI_RUN;
                }
            }

            if (lastAniIndex != newAniIndex)
            {
				ani = new Animation(Game1.Instance.gameData.animations[getPlayerAniIndex(newAniIndex)]);
				ani.start();

				if (newAniIndex == ANI_DIE)
				{
					glowAni = null;
				}
				else
				{
					glowAni = new Animation(Game1.Instance.gameData.animations[getPlayerGlowAniIndex(newAniIndex)]);
					glowAni.start();
				}
				lastAniIndex = newAniIndex;
            }
        }

        //HACKJEFFGIFFEN should abstract into a Player composite object, which has a known keybind, input tuning, and PhysicsSprite
        public override void input(KeyboardState keyboard)
        {
            bool m_traction = m_position.Y + height == 240;
            if ( m_traction )
            {
                if (keyboard.IsKeyDown(Keys.Left))
                {
                    m_acceleration.X = -250;
                }
                else if (keyboard.IsKeyDown(Keys.Right))
                {
                    m_acceleration.X = 250;
                }
                else
                {
                    m_acceleration.X = 0;
                    m_velocity.X *= (float)0.75; //drag
                }
            }
            else
            {
                m_acceleration.X = 0;
            }

            if ( keyboard.IsKeyDown(Keys.Space) && m_jumpAccelTime < 0.20 )
            {
                //not quite right yet...need non-linear acceleration applied over the jump window.
                //instant acceleration
                m_velocity.Y = -400;
                m_jumpAccelTime += m_lastDelta;
            } 
            else 
            {
                m_acceleration.Y = 1500;
                /*if ( m_traction )*/ m_jumpAccelTime = 0.0; //re-enable jump when on ground
            }

			if (keyboard.IsKeyDown(Keys.F) && !oldKeyboard.IsKeyDown(Keys.F))
			{
				fireProjectile();
			}

			oldKeyboard = keyboard;
        }

		private void fireProjectile()
		{
			double velX = m_velocity.X;
			double velY = m_velocity.Y;

			velX = 500;

			if (velX != 0 && velY != 0)
			{
				Projectile fireball = new Projectile(m_position.X, m_position.Y, velX, velY, PlayerId, PlayerGlowType);
				Game1.Instance.gameData.projectiles.Add(fireball);
			}
		}

        /*                            int trueIndex = getMapDataTrueIndex(mapData, row, col);
                     int boundFlags = (int)tileSet.bounds[trueIndex];
 */

		public int Width
		{
			get { return width; }
		}

		public int Height
		{
			get { return height; }
		}
    }
}
