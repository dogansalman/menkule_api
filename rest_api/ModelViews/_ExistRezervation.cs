using System;
using System.ComponentModel.DataAnnotations;
using rest_api.Models;
using System.Collections.Generic;

namespace rest_api.ModelViews
{
    public class _ExistRezervation
    {
        public List<Rezervations> rezervations { get; set; }
    }
}