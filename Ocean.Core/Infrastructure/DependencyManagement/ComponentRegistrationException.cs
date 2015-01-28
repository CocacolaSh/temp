﻿using System;
using System.Runtime.Serialization;

namespace Ocean.Core.Infrastructure.DependencyManagement
{
    [Serializable]
    public class ComponentRegistrationException : OceanException
    {
        public ComponentRegistrationException(string serviceName)
            : base(String.Format("Component {0} could not be found but is registered in the Ocean/engine/components section", serviceName))
        {
        }

        protected ComponentRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}