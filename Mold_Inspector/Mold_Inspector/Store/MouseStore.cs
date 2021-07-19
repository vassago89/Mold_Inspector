using Mold_Inspector.Model.Selection;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    public enum CommandState
    {
        None, Selection, Modify, Windows, Create
    }

    public  enum MouseState
    {
        None, Left, Right, Wheel
    }

    class MouseStore : BindableBase
    {
        public MouseState Mouse { get; set; }

        private CommandState _command;
        public CommandState Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        public SelectDirection SelectDirection { get; set; }

        private double _clickedX;
        public double ClickedX 
        { 
            get => _clickedX;
            set => SetProperty(ref _clickedX, value); 
        }

        public double _clickedY;
        public double ClickedY
        {
            get => _clickedY;
            set => SetProperty(ref _clickedY, value);
        }

        public double _currentX;
        public double CurrentX
        {
            get => _currentX;
            set => SetProperty(ref _currentX, value);
        }

        public double _currentY;
        public double CurrentY
        {
            get => _currentY;
            set => SetProperty(ref _currentY, value);
        }

        TeachingStore _teachingStore;
        ImageStore _imageStore;

        public MouseStore(
            TeachingStore teachingStore,
            ImageStore imageStore)
        {
            _teachingStore = teachingStore;
            _imageStore = imageStore;
        }
    }
}
