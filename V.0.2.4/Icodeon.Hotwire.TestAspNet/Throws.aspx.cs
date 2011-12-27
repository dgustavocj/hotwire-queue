using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Icodeon.Hotwire.TestAspNet
{
    public partial class Throws : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            throw new ApplicationException("This webpage throws an exception");
        }
    }
}