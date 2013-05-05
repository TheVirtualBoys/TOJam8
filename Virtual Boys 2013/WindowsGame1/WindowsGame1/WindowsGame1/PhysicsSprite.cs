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

        public override void update(GameTime frameTime)
        {
            base.update( frameTime );

            float dt = (float)(frameTime.ElapsedGameTime.Seconds + frameTime.ElapsedGameTime.Milliseconds * 0.001);
            m_velocity += m_acceleration * dt;
            Vector2 position = m_position + m_velocity * dt;
            m_lastDelta = dt;
            //HACKJEFFGIFFEN until collision is up
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
                position.Y = 240 - height;
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
                if ( m_traction ) m_jumpAccelTime = 0.0; //re-enable jump when on ground
            }
        }

        /*                            int trueIndex = getMapDataTrueIndex(mapData, row, col);
                     int boundFlags = (int)tileSet.bounds[trueIndex];
 */
    }
}
