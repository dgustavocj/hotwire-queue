using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.DTOs;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class MappingExtensions
    {
        public static EndpointDTO ToDTO(this IModuleEndpoint endpoint)
        {
            var dto = new EndpointDTO
                       {
                           Security = endpoint.Security,
                           Action = endpoint.Action,
                           Active = endpoint.Active,
                           HttpMethods = endpoint.HttpMethods,
                           MediaType = endpoint.MediaType,
                           Name = endpoint.Name,
                           UriTemplate = endpoint.UriTemplate,
                           PrivateKey = endpoint.PrivateKey,
                           TimeStampMaxAgeSeconds = endpoint.TimeStampMaxAgeSeconds
                       };
            return dto;
        }
    }
}
