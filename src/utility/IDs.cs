﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkIO.src.utility
{
    public enum IDs
    {
        #region Entities
        ENTITY_DEFAULT,
        SHOOTER,
        PROJECTILE,
        COMPOSITE,
        CIRCULAR_COMPOSITE,
        SPIKE,
        LINK_COMPOSITE,
        TRIANGULAR_EQUAL_COMPOSITE,
        TRIANGULAR_90ANGLE_COMPOSITE,

    EMPTY_LINK,
        CLOUD,
        SUN,
        ENTITY_BUTTON,
        #endregion
        
        #region Controllers
        Player,
        CONTROLLER_DEFAULT,
        CHASER_AI,
        CIRCULAR_AI,
        INDECISIVE_AI,
        RANDOM_AI,
        #endregion
        


    }
}
