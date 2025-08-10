
        public class SchedulerConfig
        {
            /// <summary>
            /// Cron expression for scheduling (optional if using interval or once)
            /// </summary>
            public string? Cron { get; set; }
            /// <summary>
            /// Type: auto (recurring), once (one-time), manual (no schedule)
            /// </summary>
            public string Type { get; set; } = "manual";
            /// <summary>
            /// For interval-based scheduling (e.g., every X minutes/hours)
            /// </summary>
            public int? IntervalMinutes { get; set; }
            /// <summary>
            /// Scheduled date/time for one-time jobs
            /// </summary>
            public DateTime? ScheduledDateTime { get; set; }
            /// <summary>
            /// Optional: End date/time for recurring jobs
            /// </summary>
            public DateTime? EndDateTime { get; set; }
        }

        public class SharePointExportResult
        {
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = "sharepoint";
            public string Url { get; set; } = string.Empty;
            public string Scope { get; set; } = string.Empty;
            public List<SharePointExportItem> Items { get; set; } = new();
            public SchedulerConfig? Scheduler { get; set; }
        }

    public class SharePointExportItem
    {
        public string Type { get; set; } = string.Empty; // page/doc lib/folder/file
        public string Path { get; set; } = string.Empty;
    }



namespace BlazorTodoApp.Services
{
    public class SharePointItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsFolder { get; set; }
        public List<SharePointItem> Children { get; set; } = new();
    }

    public interface ISharePointService
    {
        Task<List<SharePointItem>> GetSitePagesAsync(string siteUrl);
        Task<List<SharePointItem>> GetDocumentLibrariesAsync(string siteUrl);
        Task<List<SharePointItem>> GetLibraryItemsAsync(string siteUrl, string libraryId);
        /// <summary>
        /// Gets the children of a folder or library item by its ID. For libraries, libraryId is the root folder.
        /// </summary>
        Task<List<SharePointItem>> GetFolderChildrenAsync(string siteUrl, string libraryId, string folderId);
        /// <summary>
        /// Gets a single item by its ID (optional, for extensibility).
        /// </summary>
        Task<SharePointItem?> GetItemByIdAsync(string siteUrl, string libraryId, string itemId);
    }

    public class StubSharePointService : ISharePointService
    {
        // Hardcoded data for simulation
        private readonly Dictionary<string, List<SharePointItem>> _libraryRoots = new()
        {
            ["lib1"] = new List<SharePointItem>
            {
                new SharePointItem { Id = "lib1-folder1", Name = "FolderA", IsFolder = true },
                new SharePointItem { Id = "lib1-file1", Name = "RootFile1.pdf", IsFolder = false },
                new SharePointItem { Id = "lib1-file2", Name = "RootFile2.docx", IsFolder = false },
            },
            ["lib2"] = new List<SharePointItem>
            {
                new SharePointItem { Id = "lib2-folder1", Name = "FolderB", IsFolder = true },
                new SharePointItem { Id = "lib2-file1", Name = "SharedFile1.pptx", IsFolder = false },
            },
            ["lib3"] = new List<SharePointItem>
            {
                new SharePointItem { Id = "lib3-file1", Name = "FileOnly1.txt", IsFolder = false },
                new SharePointItem { Id = "lib3-file2", Name = "FileOnly2.txt", IsFolder = false },
            },
            ["lib4"] = new List<SharePointItem>()
        };

        private readonly Dictionary<string, List<SharePointItem>> _folderChildren = new()
        {
            ["lib1-folder1"] = new List<SharePointItem>
            {
                new SharePointItem { Id = "lib1-folder1-file1", Name = "FileA1.docx", IsFolder = false },
                new SharePointItem { Id = "lib1-folder1-file2", Name = "FileA2.xlsx", IsFolder = false },
                new SharePointItem { Id = "lib1-folder1-folder2", Name = "SubFolderA1", IsFolder = true },
                new SharePointItem { Id = "lib1-folder1-folder3", Name = "EmptySubFolder", IsFolder = true },
            },
            ["lib1-folder1-folder2"] = new List<SharePointItem>
            {
                new SharePointItem { Id = "lib1-folder1-folder2-file1", Name = "FileA1-1.txt", IsFolder = false }
            },
            ["lib1-folder1-folder3"] = new List<SharePointItem>(),
            ["lib2-folder1"] = new List<SharePointItem>
            {
                new SharePointItem { Id = "lib2-folder1-file1", Name = "FileB1.docx", IsFolder = false }
            }
        };

        public Task<List<SharePointItem>> GetSitePagesAsync(string siteUrl)
        {
            return Task.FromResult(new List<SharePointItem>
            {
                new SharePointItem { Id = "1", Name = "Home.aspx", IsFolder = false },
                new SharePointItem { Id = "2", Name = "About.aspx", IsFolder = false },
            });
        }

        public Task<List<SharePointItem>> GetDocumentLibrariesAsync(string siteUrl)
        {
            return Task.FromResult(new List<SharePointItem>
            {
                new SharePointItem { Id = "lib1", Name = "Documents", IsFolder = true },
                new SharePointItem { Id = "lib2", Name = "Shared Documents", IsFolder = true },
                new SharePointItem { Id = "lib3", Name = "Files Only", IsFolder = true },
                new SharePointItem { Id = "lib4", Name = "Empty Library", IsFolder = true },
            });
        }

        public Task<List<SharePointItem>> GetLibraryItemsAsync(string siteUrl, string libraryId)
        {
            // Return root items for the library
            if (_libraryRoots.TryGetValue(libraryId, out var items))
                return Task.FromResult(items);
            return Task.FromResult(new List<SharePointItem>());
        }

        public Task<List<SharePointItem>> GetFolderChildrenAsync(string siteUrl, string libraryId, string folderId)
        {
            // Return children for a folder
            if (_folderChildren.TryGetValue(folderId, out var children))
                return Task.FromResult(children);
            return Task.FromResult(new List<SharePointItem>());
        }

        public Task<SharePointItem?> GetItemByIdAsync(string siteUrl, string libraryId, string itemId)
        {
            // Search in root
            if (_libraryRoots.TryGetValue(libraryId, out var rootItems))
            {
                var found = rootItems.FirstOrDefault(i => i.Id == itemId);
                if (found != null) return Task.FromResult<SharePointItem?>(found);
            }
            // Search in folders
            if (_folderChildren.TryGetValue(itemId, out var folderItems))
            {
                // Return the folder itself as a container
                return Task.FromResult<SharePointItem?>(new SharePointItem { Id = itemId, Name = itemId, IsFolder = true, Children = folderItems });
            }
            // Search in all folder children
            foreach (var kvp in _folderChildren)
            {
                var found = kvp.Value.FirstOrDefault(i => i.Id == itemId);
                if (found != null) return Task.FromResult<SharePointItem?>(found);
            }
            return Task.FromResult<SharePointItem?>(null);
        }
    }
}
