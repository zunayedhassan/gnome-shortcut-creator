using System;
using System.IO;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public  Entry             ApplicationNameEntry  = new Entry();
    public  Entry             CommandEntry          = new Entry();
    public  Button            BrowseAppButton       = new Button("Browse");
    public  Entry             IconEntry             = new Entry();
    public  Button            BrowseIconButton      = new Button("Browse");
    public  Entry             CommentEntry          = new Entry();
    public  Image             IconPreviewImage      = new Image();
    public  Button            SaveButton            = new Button("Save");

    private Gdk.Pixbuf        _pixBuf               = null;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        this._initializeData();
        this._initializeGUI();
        this._initializeEvents();
        this._initializeFinally();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    private void _initializeData() 
    {
        
    }

    private void _initializeGUI()
    {
        this.Title = com.appiomatic.GnomeShortcutCreator.Settings.WINDOW_TITLE;
        this.SetDefaultSize(com.appiomatic.GnomeShortcutCreator.Settings.WINDOW_WIDTH, com.appiomatic.GnomeShortcutCreator.Settings.WINDOW_HEIGHT);

        const int columnSpacing = 10;

        Table root = new Table(4, 4, true);

        root.SetColSpacing(1, columnSpacing);
        root.SetColSpacing(2, columnSpacing);
        root.SetColSpacing(3, columnSpacing);
        root.SetColSpacing(4, columnSpacing);

        root.Attach(new Label("Application Name"), 0, 1, 0, 1);
        root.Attach(this.ApplicationNameEntry, 1, 2, 0, 1);
        root.Attach(new Label("Command"), 0, 1, 1, 2);
        root.Attach(this.CommandEntry, 1, 2, 1, 2);
        root.Attach(this.BrowseAppButton, 2, 3, 1, 2);
        root.Attach(new Label("Icon"), 0, 1, 2, 3);
        root.Attach(this.IconEntry, 1, 2, 2, 3);
        root.Attach(this.BrowseIconButton, 2, 3, 2, 3);
        root.Attach(this.IconPreviewImage, 3, 4, 2, 3);
        root.Attach(new Label("Comment"), 0, 1, 3, 4);
        root.Attach(this.CommentEntry, 1, 2, 3, 4);

        this.IconEntry.IsEditable = false;

        try
        {
            this._pixBuf = new Gdk.Pixbuf("icons/thumbnails.png");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

        this.IconPreviewImage.Pixbuf = this._pixBuf;

        VBox layout = new VBox(false, 5);
        layout.PackStart(root, true, true, 5);

        this.SaveButton.SetSizeRequest(100, 50);

        HBox bottomLayout = new HBox(false, 5);
        bottomLayout.PackEnd(this.SaveButton, true, false, 5);

        layout.PackEnd(bottomLayout, false, false, 5);

        this.Add(layout);

        this.SetIconFromFile("icons/icon.png");
    }

    private void _initializeEvents()
    {
        this.BrowseAppButton.Clicked  += this.OnCommandAppButtonClicked;
        this.BrowseIconButton.Clicked += this.OnIconButtonClicked;
        this.SaveButton.Clicked       += this.OnSaveButtonClicked;
    }

    private void _initializeFinally()
    {
                                                                                                                             
    }

    public void OnCommandAppButtonClicked(object sender, EventArgs args) 
    {
        FileChooserDialog filechooser = new FileChooserDialog("Choose the application to create a shortcut of",
            this,
            FileChooserAction.Open,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept
        );

        if (filechooser.Run() == (int) ResponseType.Accept) 
        {
            string selectedFileName = filechooser.Filename;
            this.CommandEntry.Text = selectedFileName;
        }

        filechooser.Destroy();
    }

    public void OnIconButtonClicked(object sender, EventArgs args)
    {
        FileChooserDialog filechooser = new FileChooserDialog("Choose the icon of your shortcut",
            this,
            FileChooserAction.Open,
            "Cancel", ResponseType.Cancel,
            "Open", ResponseType.Accept
        );

        if (filechooser.Run() == (int) ResponseType.Accept)
        {
            string selectedFileName = filechooser.Filename;

            if (selectedFileName.ToLower().EndsWith(".jpg") || selectedFileName.ToLower().EndsWith(".jpeg") || selectedFileName.ToLower().EndsWith(".png") || selectedFileName.ToLower().EndsWith(".gif") || selectedFileName.ToLower().EndsWith(".bmp") || selectedFileName.ToLower().EndsWith(".ico"))
            {
                Gdk.Pixbuf pixBuf = null;

                try
                {
                    pixBuf = new Gdk.Pixbuf(selectedFileName);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                this._applyIcon(pixBuf, selectedFileName);
            }
            else {
                this._applyIcon(this._pixBuf, "");
            }
        }
        else 
        {
            this._applyIcon(this._pixBuf, "");
        }

        filechooser.Destroy();
    }

    private void _applyIcon(Gdk.Pixbuf pixbuf, string iconPath)
    {
        this.IconPreviewImage.Pixbuf = pixbuf;
        this.IconEntry.Text = iconPath;
    }

    public void OnSaveButtonClicked(object sender, EventArgs args)
    {
        FileChooserDialog filechooser = new FileChooserDialog("Save your shortcut",
            this,
            FileChooserAction.Save,
            "Cancel", ResponseType.Cancel,
            "Save", ResponseType.Accept
        );

        if (filechooser.Run() == (int) ResponseType.Accept)
        {
            string selectedFileName = filechooser.Filename;

            if (!selectedFileName.Trim().EndsWith(".desktop"))
            {
                selectedFileName += ".desktop";
            }

            string[] lines = new string[] {
                "[Desktop Entry]",
                "Type=Application",
                "Encoding=UTF-8",
                "Name=" + this.ApplicationNameEntry.Text.Trim(),
                "Comment=" + this.CommentEntry.Text.Trim(),
                "Exec=" + this.CommandEntry.Text.Trim(),
                "Icon=" + this.IconEntry.Text.Trim(),
                "Terminal=false"
            };

            using (StreamWriter file = new StreamWriter(selectedFileName))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line);
                }

                file.Close();
            }
        }

        filechooser.Destroy();
    }
}
