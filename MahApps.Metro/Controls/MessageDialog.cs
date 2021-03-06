﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public class MessageDialog : Control
    {
        private const string PART_AffirmativeButton = "PART_AffirmativeButton";
        private const string PART_NegativeButton = "PART_NegativeButton";

        private Button AffirmativeButton = null;
        private Button NegativeButton = null;

        static MessageDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
        }
        internal MessageDialog()
        {
        }

        internal Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            this.Focus();

            //kind of acts like a selective 'IsDefault' mechanism.
            if (ButtonStyle == MessageDialogStyle.Affirmative)
                AffirmativeButton.Focus();
            else if (ButtonStyle == MessageDialogStyle.AffirmativeAndNegative)
                NegativeButton.Focus();

            TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            Action cleanUpHandlers = () =>
            {
                NegativeButton.Click -= negativeHandler;
                AffirmativeButton.Click -= affirmativeHandler;

                NegativeButton.KeyDown -= negativeKeyHandler;
                AffirmativeButton.KeyDown -= affirmativeKeyHandler;
            };


            negativeKeyHandler = new KeyEventHandler((sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Negative);
                    }
                });

            affirmativeKeyHandler = new KeyEventHandler((sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                });


            negativeHandler = new RoutedEventHandler((sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);

                    e.Handled = true;
                });

            affirmativeHandler = new RoutedEventHandler((sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);

                    e.Handled = true;
                });

            NegativeButton.KeyDown += negativeKeyHandler;
            AffirmativeButton.KeyDown += affirmativeKeyHandler;

            NegativeButton.Click += negativeHandler;
            AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(MessageDialogStyle), typeof(MessageDialog), new PropertyMetadata(MessageDialogStyle.Affirmative));

        public MessageDialogStyle ButtonStyle
        {
            get { return (MessageDialogStyle)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public string AffirmativeButtonText
        {
            get { return (string)this.GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }
        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            AffirmativeButton = GetTemplateChild(PART_AffirmativeButton) as Button;
            NegativeButton = GetTemplateChild(PART_NegativeButton) as Button;

            base.OnApplyTemplate();
        }
    }
    public enum MessageDialogResult
    {
        Negative = 0,
        Affirmative = 1,
    }

    public enum MessageDialogStyle
    {
        /// <summary>
        /// Just "OK"
        /// </summary>
        Affirmative = 0,
        /// <summary>
        /// "OK" and "Cancel"
        /// </summary>
        AffirmativeAndNegative = 1,
    }

    public class MessageDialogSettings
    {
        internal MessageDialogSettings()
        {
            AffirmativeButtonText = "OK";
            NegativeButtonText = "Cancel";
        }

        public string AffirmativeButtonText { get; set; }
        public string NegativeButtonText { get; set; }
    }
}
