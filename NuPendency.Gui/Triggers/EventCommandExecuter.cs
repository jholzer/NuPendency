using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NuPendency.Gui.Triggers
{
    public class EventCommandExecuter : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandExecuter), new PropertyMetadata(null));

        public static readonly DependencyProperty EventArgsConverterParameterProperty =
            DependencyProperty.Register("EventArgsConverterParameter", typeof(object), typeof(EventCommandExecuter), new PropertyMetadata(null));

        public EventCommandExecuter()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public EventCommandExecuter(CultureInfo culture)
        {
            Culture = culture;
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public CultureInfo Culture { get; set; }

        public IValueConverter EventArgsConverter { get; set; }

        public object EventArgsConverterParameter
        {
            get { return GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            var cmd = Command;

            if (cmd != null)
            {
                var param = parameter;

                if (EventArgsConverter != null)
                {
                    param = EventArgsConverter.Convert(parameter, typeof(object), EventArgsConverterParameter, CultureInfo.InvariantCulture);
                }

                if (cmd.CanExecute(param))
                {
                    cmd.Execute(param);
                }
            }
        }
    }
}