﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadersConnect.Web.Dtos
{
    public class GenericResponse<T> where T : class
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
    }
}