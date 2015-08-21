using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindAllHudgeFiles
{
    /// <summary>
    /// 代表找到的一个文件
    /// </summary>
    public class FoundFile : INotifyPropertyChanged
    {
       
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    OnPropertyChanged("Name");
                    _name = value;
                }

            }
        }
        private long _length;
        public long Length
        {
            get
            {
                return _length;
            }
            set
            {
                if (_length != value)
                {
                    OnPropertyChanged("Length");
                    _length = value ;
                }
            }
        }
        private string _location;
        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location != value)
                {
                    OnPropertyChanged("Location");
                    _location = value;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
