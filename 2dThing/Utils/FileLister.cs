using System;
using SFML.Graphics;
using System.IO;
using System.Collections.Generic;
using SFML.Window;

namespace _2dThing {
	public class FileLister {
		
		public static int DIRECTORY = 0;
		public static int FILE = 1;
		public static int NEWFILE = 2;
		public static int NEWDIRECTORY = 3;
		
		private DirectoryInfo current;
		private List<DirectoryInfo> directories;
		private List<FileInfo> files;
		private String filter;
		private int cursor = 0;
		private int length = 0;
		private bool load = true;
		private int editMode = 0;
		private string newName = "";
		
		public FileLister(String path, bool load) : this(path, load, "*") {
			
		}
		
		public FileLister(String path, bool load, String filter){
			this.filter = filter;
			this.load = load;
			SetPath(path);
		}
		
		public void SetPath(String path) {
			SetDirectory( new DirectoryInfo(path));			
		}
		
		public void SetDirectory(DirectoryInfo directory){
			current = directory;
			directories = new List<DirectoryInfo>(current.EnumerateDirectories());
			files = new List<FileInfo>(current.EnumerateFiles(filter));			
			length = directories.Count + files.Count;
			
			if(!load)
				length += 2;
		}
		
		//TODO Clean with a drawtext method
		public void Draw(RenderTarget rt){
			float maxX = 0;
			Vector2f position = new Vector2f(2,0);
			Text text = new Text("", Screen.ARIAL);
			
			if(current == null)
				text.DisplayedString = "Logical Drives";
			else
				text.DisplayedString = current.FullName;
			
			text.CharacterSize = 12;
			rt.Draw(text);
			position.Y += text.GetRect().Height + 4;
			float startY = position.Y;
			
			int index = 0;
			foreach(DirectoryInfo d in directories){
				
				if(index == cursor)
					text.Color = Color.Yellow;
				else
					text.Color = Color.White;
				if(current == null)
					text.DisplayedString = d.Name;	
				else
					text.DisplayedString = "\\" + d.Name;
				
				if(text.GetRect().Top + text.GetRect().Height > rt.Height - 10){
					position.X += maxX + 2;
					position.Y = startY;
					maxX = 0;					
				}
					
				text.Position = position;
				rt.Draw(text);
				position.Y += text.GetRect().Height + 4;
				index++;
				
				maxX = Math.Max(maxX, text.GetRect().Width);
			}
			
			if(!load && current != null){
				
				if(index == cursor)
					text.Color = Color.Yellow;
				else
					text.Color = Color.White;
				
				if(editMode == NEWDIRECTORY)
					text.DisplayedString = newName + "_";
				else
					text.DisplayedString = "New directory";
				
				if(text.GetRect().Top + text.GetRect().Height > rt.Height - 10){
					position.X += maxX + 2;
					position.Y = startY;
					maxX = 0;					
				}
				
				text.Position = position;
				rt.Draw(text);
				position.Y += text.GetRect().Height + 4;
				index++;
				
				maxX = Math.Max(maxX, text.GetRect().Width);
				
			}
			
			foreach(FileInfo f in files){
				
				if(index == cursor)
					text.Color = Color.Yellow;
				else
					text.Color = Color.White;
				
				text.DisplayedString = f.Name;
				
				if(text.GetRect().Top + text.GetRect().Height > rt.Height - 10){
					position.X += maxX + 2;
					position.Y = startY;
					maxX = 0;					
				}
				
				text.Position = position;
				rt.Draw(text);
				position.Y += text.GetRect().Height + 4;
				index++;
				
				maxX = Math.Max(maxX, text.GetRect().Width);
			}
			
			if(!load && current != null){
				
				if(index == cursor)
					text.Color = Color.Yellow;
				else
					text.Color = Color.White;
				
				if(editMode == NEWFILE)
					text.DisplayedString = newName + "_";
				else
					text.DisplayedString = "New file";
				
				if(text.GetRect().Top + text.GetRect().Height > rt.Height - 10){
					position.X += maxX + 2;
					position.Y = startY;
					maxX = 0;					
				}
				
				text.Position = position;
				rt.Draw(text);
				position.Y += text.GetRect().Height + 4;
				index++;
				
				maxX = Math.Max(maxX, text.GetRect().Width);
				
			}
		}
		
		public void Parent(){
			if(current != null && current.Parent != null)
				SetDirectory(current.Parent);
			else {
				current = null;
				length = 0;
				directories.Clear();
				files.Clear();
				foreach(String drive in Directory.GetLogicalDrives()) {
					directories.Add(new DirectoryInfo(drive));
					length++;
				}
			}
				
		}
		
		public void Up(){
			cursor = (cursor + length - 1) % length;
		}
		
		public void Down(){
			cursor = (cursor + 1) % length;
		}
		
		public int Expand(){
			if(cursor < directories.Count){
				SetDirectory(directories[cursor]);
				return DIRECTORY;
			}
			else if(!load && cursor == directories.Count)
				return NEWDIRECTORY;
			else if(!load && cursor == directories.Count + 1 + files.Count)
				return NEWFILE;
			else
				return FILE;				
		}
		
		public String GetSelectedIndex(){
			if(cursor < directories.Count)
				return directories[cursor].FullName;
			else if (load)
				return files[cursor - directories.Count].FullName;
			else if (cursor == directories.Count)
				return "New directory";
			else if (cursor == directories.Count + 1 + files.Count)
				return "New file";
			else
				return files[cursor - directories.Count - 1].FullName;			
		}
		
		public void EnterEditMode(int mode){
			editMode = mode;
			Console.WriteLine(editMode);
		}
		
		public bool IsEditing(){
			return editMode != 0;
		}
		
		public void AddChar(String c){
			if (c.Equals("\b")){
				if(newName.Length > 0)
					newName = newName.Substring(0, newName.Length - 1);
			}
			else if (c.Equals("\r") || c.Equals("\n")){
				if(newName.Length > 0) {
					if(editMode == NEWDIRECTORY){
						current.CreateSubdirectory(newName);
					}
					else if(editMode == NEWFILE){
						
						if(!newName.EndsWith(".map"))
							newName += ".map";
						
						FileStream fs = File.Create(current.FullName + "/" + newName);
						fs.Close();
					}
					editMode = 0;
					newName="";
					SetDirectory(current);
				}
			}
			else
				newName += c;				
		}
		
		public void Delete(){
			if(Expand() == FILE){
				File.Delete(GetSelectedIndex());
				SetDirectory(current);
			}
		}
	}
}

