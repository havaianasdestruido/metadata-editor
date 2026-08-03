using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MetaLab.Engine;

namespace MetaLab;

public class MainForm : Form
{
    private ListView fileList;
    private DataGridView metaGrid;
    private Button btnSave;
    private Label lblStatus;
    private string selectedPath;

    public MainForm()
    {
        Text = "MetaLab - Metadata Editor";
        Width = 800; Height = 600;
        InitControls();
        LoadEngineStatus();
    }

    private void InitControls()
    {
        fileList = new ListView { View = View.List, Dock = DockStyle.Left, Width = 200 };
        fileList.SelectedIndexChanged += FileList_SelectedIndexChanged;

        metaGrid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false };
        metaGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Key", DataPropertyName = "Key", Width = 200 });
        metaGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = "Value", Width = 300 });

        btnSave = new Button { Text = "Save", Dock = DockStyle.Bottom, Height = 30 };
        btnSave.Click += BtnSave_Click;

        lblStatus = new Label { Dock = DockStyle.Bottom, Height = 20, Text = "Status: " };

        Controls.Add(metaGrid);
        Controls.Add(fileList);
        Controls.Add(btnSave);
        Controls.Add(lblStatus);
        PopulateFileList();
    }

    private void PopulateFileList()
    {
        var dir = Environment.CurrentDirectory; // start folder
        foreach (var f in System.IO.Directory.GetFiles(dir, "*.*", System.IO.SearchOption.TopDirectoryOnly))
        {
            var ext = System.IO.Path.GetExtension(f).ToLowerInvariant();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".mp4")
                fileList.Items.Add(new ListViewItem { Text = System.IO.Path.GetFileName(f), Tag = f });
        }
    }

    private void FileList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (fileList.SelectedItems.Count == 0) return;
        selectedPath = fileList.SelectedItems[0].Tag as string;
        var ext = System.IO.Path.GetExtension(selectedPath).ToLowerInvariant();
        Dictionary<string,string> meta = ext switch
        {
            ".mp4" => VideoMetadataEditor.Load(selectedPath),
            _ => ImageMetadataEditor.Load(selectedPath)
        };
        var bs = new BindingSource();
        foreach (var kv in meta) bs.Add(new { Key = kv.Key, Value = kv.Value });
        metaGrid.DataSource = bs;
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(selectedPath)) return;
        var updates = new Dictionary<string,string>();
        foreach (DataGridViewRow row in metaGrid.Rows)
        {
            if (row.IsNewRow) continue;
            var key = row.Cells[0].Value?.ToString();
            var val = row.Cells[1].Value?.ToString();
            if (!string.IsNullOrEmpty(key)) updates[key] = val ?? "";
        }
        var ext = System.IO.Path.GetExtension(selectedPath).ToLowerInvariant();
        if (ext == ".mp4") VideoMetadataEditor.Save(selectedPath, updates);
        else ImageMetadataEditor.Save(selectedPath, updates);
        lblStatus.Text = "Saved " + System.IO.Path.GetFileName(selectedPath);
    }

    private void LoadEngineStatus()
    {
        var hr = EngineProbe.ProbeMetadataSys();
        lblStatus.Text = $"MetadataSys probe: 0x{hr:X8}";
    }
}
