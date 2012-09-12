using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Railz
{
    class Player
    {

        #region Variables

        AnimatedSprite asSprite;
        int iX = 604;
        int iY = 260;
        int iFacing = 0;
        bool bThrusting = false;
        int iScrollRate = 0;
        int iShipAccelerationRate = 1;
        int iShipVerticalMoveRate = 3;
        float fSpeedChangeCount = 0.0f;
        float fSpeedChangeDelay = 0.01f;
        float fVerticalChangeCount = 0.0f;
        float fVerticalChangeDelay = 0.1f;
        
        #endregion

        #region Properties

        public int X
        {
            get { return iX; }
            set { iX = value; }
        }

        public int Y
        {
            get { return iY; }
            set { iY = value; }
        }

        public int Facing

        #endregion

        #region Constructor



        #endregion

        #region Methods



        #endregion
    }
}
