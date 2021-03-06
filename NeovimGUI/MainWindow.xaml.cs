﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MsgPack;
using Neovim;
using System.Configuration;
using System.IO;
using NeovimTerminal;

namespace NeovimGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //_renderProperties = new RenderProperties();
            //_term = new Terminal(_renderProperties);
            //TerminalContainer.Children.Add(_term);
        }

        private NeovimClient _neovim;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if(File.Exists(ConfigurationManager.AppSettings["neovim_path"]))
                {
                    _neovim = new NeovimClient(ConfigurationManager.AppSettings["neovim_path"]);
                    _neovim.Redraw += OnRedraw;
                    _neovim.ui_attach(80, 24, true);

                    this.KeyDown += term_KeyDown;
                    Grid.SizeChanged += Window_SizeChanged;
                }
                else
                {
                    MessageBox.Show("Couldn't find the neovim executable at location [" +
                        ConfigurationManager.AppSettings["neovim_path"] +
                        "]\r\nIt may be missing, inaccessible or you may need to change Neovim.cs XML configuration.");
                    Environment.Exit(1);
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show("There was an error starting the NeovimClient.\r\n" + ex.ToString());
                Environment.Exit(1);
            }
        }

        private delegate void UpdateTerminalWindowDelegate(Action<TerminalTst> message);
        private void UpdateTerminal(Action<TerminalTst> message)
        {
            if (!_term.Dispatcher.CheckAccess())
            {
                UpdateTerminalWindowDelegate update = UpdateTerminal;
                _term.Dispatcher.BeginInvoke(update, message);
            }
            else
            {
                message(_term);
            }
        }

        private void OnRedraw(object sender, NeovimRedrawEventArgs e)
        {
            foreach (var f in e.Functions)
            {
                var list = f.AsList();
                string function = list[0].AsString(Encoding.Default);


                IList<IList<MessagePackObject>> args = new List<IList<MessagePackObject>>();
                for (int i = 1; i < list.Count; i++)
                {
                    args.Add(list[i].AsList());
                }

                switch (function)
                {
                    case "clear":
                        UpdateTerminal((t) => t.Clear());
                        break;
                    case "resize":
                        UpdateTerminal((t) => t.Resize(args[0][1].AsInt32(), args[0][0].AsInt32()));
                        break;

                    case "update_fg":
                        break;

                    case "update_bg":
                        break;

                    case "highlight_set":
                        System.Drawing.Color fg = System.Drawing.Color.White;
                        bool bold = false;
                        bool italic = false;

                        foreach (var arg in args)
                        {
                            var dict = arg[0].AsDictionary();

                            foreach (var entry in dict)
                            {
                                var str = entry.Key.AsString(Encoding.Default);
                                if (str == "foreground")
                                {
                                    uint c = entry.Value.AsUInt32();
                                    byte r = (byte)(c >> 16);
                                    byte g = (byte)(c >> 8);
                                    byte b = (byte)(c >> 0);
                                    fg = System.Drawing.Color.FromArgb(r, g, b);
                                }
                                else if (str == "bold")
                                    bold = entry.Value.AsBoolean();
                                else if (str == "italic")
                                    italic = entry.Value.AsBoolean();

                            }
                        }

                        UpdateTerminal((t) => t.Highlight(fg, bold, italic));
                        break;

                    case "eol_clear":
                        UpdateTerminal((t) => t.ClearToEnd());
                        break;

                    case "set_title":
                        this.Title = args[0][0].AsString(Encoding.Default);
                        break;

                    case "put":
                        List<byte> bytes = new List<byte>();

                        foreach (var arg in args)
                            bytes.AddRange(arg[0].AsBinary());

                        UpdateTerminal((t) => t.PutText(Encoding.Default.GetString(bytes.ToArray())));
                        break;

                    case "cursor_goto":
                        UpdateTerminal((t) => t.MoveCaret(args[0][0].AsInt32(), args[0][1].AsInt32()));
                        break;

                    case "scroll":
                        UpdateTerminal((t) => t.Scroll(args[0][0].AsSByte()));
                        break;

                    case "set_scroll_region":
                        break;

                    case "normal_mode":
                        UpdateTerminal((t) => t.Caret.Width = t.CellWidth);
                        break;

                    case "insert_mode":
                        UpdateTerminal((t) => t.Caret.Width = 3);
                        break;

                    case "busy_start":
                        break;

                    case "busy_stop":
                        break;

                    case "mouse_on":
                        break;

                    case "mouse_off":
                        break;
                }
            }
            UpdateTerminal((t) => t.SwapBuffers());
        }

        private void term_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.LeftAlt ||
                e.Key == Key.RightAlt || e.Key == Key.CapsLock || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                return;

            string keys = Input.Encode(e.Key);
            if (keys != null)
                _neovim.vim_input(keys);

            e.Handled = true;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int rows = (int)Math.Round(e.NewSize.Height / _term.CellHeight);
            int columns = (int) Math.Round(e.NewSize.Width/_term.CellWidth);

            _term.Resize(rows, columns);
//                _neovim.ui_try_resize(columns, rows);
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //TODO: this blocks the main thread and seems to hang when changes are unsaved :(
            //_neovim.vim_command(":qa");
            _neovim.vim_command(":qa!");
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _neovim.vim_command(":w");
        }
    }
}
