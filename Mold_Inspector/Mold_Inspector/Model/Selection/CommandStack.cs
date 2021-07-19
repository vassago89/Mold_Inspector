using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Model.Selection
{
    public interface ICommandStack
    {
        bool CanRedo { get; }
        bool CanUndo { get; }

        void Redo();
        void Undo();

        void Push(ICommand command);
        void Clear();
    }

    ///   <summary>
    ///  Stack with capacity, bottom items beyond the capacity are discarded.
    ///   </summary>
    ///   <typeparam name="T"></typeparam>
    [Serializable]
    public class RoundStack<T>
    {
        private T[] items;  //  items.Length is Capacity + 1

        //  top == bottom ==> full
        private int top = 1;
        private int bottom = 0;

        ///   <summary>
        ///  Gets if the  <see cref="RoundStack&lt;T&gt;"/>  is full.
        ///   </summary>
        public bool IsFull
        {
            get
            {
                return top == bottom;
            }
        }

        ///   <summary>
        ///  Gets the number of elements contained in the  <see cref="RoundStack&lt;T&gt;"/> .
        ///   </summary>
        public int Count
        {
            get
            {
                int count = top - bottom - 1;
                if (count < 0)
                    count += items.Length;
                return count;
            }
        }

        ///   <summary>
        ///  Gets the capacity of the  <see cref="RoundStack&lt;T&gt;"/> .
        ///   </summary>
        public int Capacity
        {
            get
            {
                return items.Length - 1;
            }
        }

        ///   <summary>
        ///  Creates  <see cref="RoundStack&lt;T&gt;"/>  with given capacity
        ///   </summary>
        ///   <param name="capacity"></param>
        public RoundStack(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(" Capacity need to be at least 1 ");
            items = new T[capacity + 1];
        }

        ///   <summary>
        ///  Removes and returns the object at the top of the  <see cref="RoundStack&lt;T&gt;"/> .
        ///   </summary>
        ///   <returns></returns>
        public T Pop()
        {
            if (Count > 0)
            {
                T removed = items[top];
                items[top--] = default(T);
                if (top < 0)
                    top += items.Length;
                return removed;
            }
            else
                throw new InvalidOperationException(" Cannot pop from emtpy stack ");
        }

        ///   <summary>
        ///  Inserts an object at the top of the  <see cref="RoundStack&lt;T&gt;"/> .
        ///   </summary>
        ///   <param name="item"></param>
        public void Push(T item)
        {
            if (IsFull)
            {
                bottom++;
                if (bottom >= items.Length)
                    bottom -= items.Length;
            }
            if (++top >= items.Length)
                top -= items.Length;
            items[top] = item;
        }

        ///   <summary>
        ///  Returns the object at the top of the  <see cref="RoundStack&lt;T&gt;"/>  without removing it.
        ///   </summary>
        public T Peek()
        {
            return items[top];
        }

        ///   <summary>
        ///  Removes all the objects from the  <see cref="RoundStack&lt;T&gt;"/> .
        ///   </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = default(T);
                }
                top = 1;
                bottom = 0;
            }
        }

    }

    public class CommandStack : BindableBase, ICommandStack
    {

        private RoundStack<ICommand> _redoStack;
        private RoundStack<ICommand> _undoStack;

        private bool _canRedo;
        public bool CanRedo
        {
            get => _canRedo;
            set => SetProperty(ref _canRedo, value);
        }

        private bool _canUndo;
        public bool CanUndo
        {
            get => _canUndo;
            set => SetProperty(ref _canUndo, value);
        }

        public CommandStack()
        {
            _redoStack = new RoundStack<ICommand>(100);
            _undoStack = new RoundStack<ICommand>(100);
        }

        public void Push(ICommand command)
        {
            _redoStack.Clear();
            _undoStack.Push(command);

            Refresh();
        }

        public void Redo()
        {
            if (CanRedo == false)
                return;

            var command = _redoStack.Pop();
            command.Redo();
            _undoStack.Push(command);

            Refresh();
        }

        public void Undo()
        {
            if (CanUndo == false)
                return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            Refresh();
        }

        private void Refresh()
        {
            CanUndo = _undoStack.Count > 0;
            CanRedo = _redoStack.Count > 0;
        }

        public void Clear()
        {
            _redoStack.Clear();
            _undoStack.Clear();
            Refresh();
        }
    }
}
