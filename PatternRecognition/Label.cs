using System;
using System.Collections.Generic;
using System.Text;

namespace PatternRecognition
{
    class Label
    {
        public Label(int Name)
        {
            this.Name = Name;
            this.Root = this;
        }

        public int Name { get; set; }

        public Label Root { get; set; }

        internal Label GetRoot()
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
    }
}