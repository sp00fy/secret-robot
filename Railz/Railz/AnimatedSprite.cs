using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Railz
{
    class AnimatedSprite
    {
        #region Variables

        Texture2D t2dTexture;

        float fFrameRate = 0.03f;
        float fElapsed = 0.0f;

        int iFrameOffsetX = 0;
        int iFrameOffsetY = 0;
        int iFramewidth = 32;
        int iFrameHeight = 32;

        int iFrameCount = 1;
        int iCurrentFrame = 0;
        int iScreenX = 0;
        int iScreenY = 0;

        bool bAnimating = true;

        #endregion

        #region Constructor

        public AnimatedSprite(
            Texture2D texture,
            int FrameOffsetX,
            int FrameOffsetY,
            int FrameWidth,
            int FrameHeight,
            int FrameCount)
        {
            t2dTexture = texture;
            iFrameOffsetX = FrameOffsetX;
            iFrameOffsetY = FrameOffsetY;
            iFramewidth = FrameWidth;
            iFrameHeight = FrameHeight;
            iFrameCount = FrameCount;
        }

        #endregion

        #region Properties

        public int X
        {
            get { return iScreenX; }
            set { iScreenX = value; }
        }

        public int Y
        {
            get { return iScreenY; }
            set { iScreenY = value; }
        }

        public int Frame
        {
            get { return iCurrentFrame; }
            set { iCurrentFrame = (int)MathHelper.Clamp(value, 0, iFrameCount); }
        }

        public float FrameLength
        {
            get { return fFrameRate; }
            set { fFrameRate = (float)Math.Max(value, 0f); }        
        }

        public bool IsAnimating
        {
            get { return bAnimating; }
            set { bAnimating = value; }
        }

        
        #endregion

        #region Methods

        public Rectangle GetSourceRect()
        {
            return new Rectangle(
                iFrameOffsetX + (iFramewidth * iCurrentFrame),
                iFrameOffsetY,
                iFramewidth,
                iFrameHeight);
        }

        // The below utilizes the game's current GameTime in order to call the games'
        // Update & Draw routines over and over in order to animate the sprites.      
        // Mod is used below because of the below example:
        // Take a 16 frame animation, starting from frame 0...
        // 16/16 = 1 with remainder of 0 so...
        // iCurrentFrameCount is 1 so...
        // iCurrentFrame gets set to 0
        // Doing the above means never having to do an if..else check

        public void Update(GameTime gametime)
        {
            if (bAnimating)
            {
                // Accumulate elapsed time...
                fElapsed += (float)gametime.ElapsedGameTime.TotalSeconds;

                // Until it passes our frame length
                if (fElapsed > fFrameRate)
                {
                    // Increment the current frame, wrapping back to 0 at iFrameCount
                    iCurrentFrame = (iCurrentFrame + 1) % iFrameCount;

                    // Reset the elapsed frame time.
                    fElapsed = 0.0f;
                }
            }
        }

        public void Draw(
            SpriteBatch spriteBatch,
            int XOffset,
            int YOffset,
            bool NeedBeginEnd)
        {
            if (NeedBeginEnd)
                spriteBatch.Begin();

            spriteBatch.Draw(
                t2dTexture,
                new Rectangle(
                    iScreenX + XOffset,
                    iScreenY + YOffset,
                    iFramewidth,
                    iFrameHeight),
                    GetSourceRect(),
                    Color.White);

            if (NeedBeginEnd)
                spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset)
        {
            Draw(spriteBatch, XOffset, YOffset, true);
        }

        #endregion

    }
}
