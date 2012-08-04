using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Label
    {
        #region Public Properties

        public int Name { get; set; }

        public Label Root { get; set; } 

        #endregion

        #region Constructor

        public Label(int Name)
        {
            this.Name = Name;
            this.Root = this;
        } 

        #endregion

        #region Public Methods

        public Label GetRoot()
        {
            if (this.Root != this)
            {
                this.Root = this.Root.GetRoot();
            }
            return this.Root;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            Label other = (Label)obj;
            return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return this.Name;
        } 

        #endregion
    }
}