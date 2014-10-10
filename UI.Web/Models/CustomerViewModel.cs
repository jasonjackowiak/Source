using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UI.Web.Models
{

    public class CustomerViewModel
    {
            //[Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            //[Required]
            [Display(Name = "Organisation Title")]
            public string Name { get; set; }

            public List<CustomerViewModel> customerViewModel { get; set; }
        }
}