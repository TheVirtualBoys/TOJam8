using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    class PhysicsSprite : Sprite
    {
        private const int width = 29;
        private const int height = 34;

        Vector2 m_position;
        Vector2 m_velocity; //px / s
        Vector2 m_acceleration; //px / s / s
        double m_lastDelta;
        double m_jumpAccelTime;
        bool m_jumping;

        public PhysicsSprite(AnimationData aniData)
            : base( aniData)
            {
                m_position = new Vector2( 128, 120);
                m_velocity = m_acceleration = new Vector2(0, 0);
                m_lastDelta = m_jumpAccelTime = 0.0;
                m_jumping = true; //flying start
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
            bool localCollides = Collides( m_position + offset, position + offset + new Vector2(1, 1), out hitPos ) != TileSet.Bounds.BOUNDS_NONE;
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
            bool localCollides = Collides(m_position + offset, position + offset, out hitPos) != TileSet.Bounds.BOUNDS_NONE;
            if (localCollides)
            {
                outY = Math.Max(y, hitPos.Y);
                collides |= localCollides;
            }
            else outY = y;
            return collides;
        }

        public bool CollisionRightHelper(bool pastCollides, Vector2 offset, Vector2 position, bool isSlash, out bool outIsSlash, bool isBSlash, out bool outIsBSlash)
        {
            Vector2 hitPos;
            TileSet.Bounds bound = Collides(m_position + offset, position + offset + new Vector2(1, 0), out hitPos);

            outIsSlash = isSlash | (bound & TileSet.Bounds.BOUNDS_SLASH) == TileSet.Bounds.BOUNDS_SLASH;
            outIsBSlash = isBSlash | (bound & TileSet.Bounds.BOUNDS_BSLASH) == TileSet.Bounds.BOUNDS_BSLASH;
            bool collided = bound != TileSet.Bounds.BOUNDS_NONE && !isSlash && !isBSlash;  //hit something which isn't a slash and isnt a backslash
            return pastCollides | collided;
        }

        public override void update(GameTime frameTime)
        {
            base.update(frameTime);

            float dt = (float)(frameTime.ElapsedGameTime.Seconds + frameTime.ElapsedGameTime.Milliseconds * 0.001);
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
                collide = CollisionDownHelper(collide, insideLeftBotOffset, position, minY, out minY);
                collide = CollisionDownHelper(collide, insideMidBotOffset, position, minY, out minY);
                collide = CollisionDownHelper(collide, insideRightBotOffset, position, minY, out minY);
                minY -= insideHeight + 1;
                if (collide)
                {
                    killVeloY = true;
                    position.Y = (float)minY;

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
                    position.Y = Math.Min(239 - height, (float)maxY + 1 );
                }
            }

            //fwd checks (in quarters top to bottom)
            {
                bool collide = false, isSlash = false, isBSlash = false;
                float insideWidth = width - 1;
                Vector2 insideRight0Offset = new Vector2(insideWidth, 0);
                Vector2 insideRight1Offset = new Vector2(insideWidth, height / 3);
                Vector2 insideRight2Offset = new Vector2(insideWidth, 2 * height / 3);
                Vector2 insideRight3Offset = new Vector2(insideWidth, height - 1);
                collide = CollisionRightHelper(collide, insideRight0Offset, position, isSlash, out isSlash, isBSlash, out isBSlash);
                collide = CollisionRightHelper(collide, insideRight1Offset, position, isSlash, out isSlash, isBSlash, out isBSlash);
                collide = CollisionRightHelper(collide, insideRight2Offset, position, isSlash, out isSlash, isBSlash, out isBSlash);
                collide = CollisionRightHelper(collide, insideRight3Offset, position, isSlash, out isSlash, isBSlash, out isBSlash);
                if (collide)
                {
                    killVeloX = true;
                    position.Y = 0; //HACKJEFFGIFFEN
                }
             /*   if ( isSlash )
                {
                    position.X = 
                */
            }


            if (killVeloX) m_velocity.X = 0;
            if (killVeloY) m_velocity.Y = 0;

            m_position = position;
            //patch back the position
            Left = (int)m_position.X;
            Top = (int)m_position.Y;
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
        }

        /*                            int trueIndex = getMapDataTrueIndex(mapData, row, col);
                     int boundFlags = (int)tileSet.bounds[trueIndex];
 */
    }
}
