using OpenKh.Kh2;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Xe.Tools;
using Xe.Tools.Models;
using Xe.Tools.Wpf.Commands;
using Xe.Tools.Wpf.Models;

namespace OpenKh.Tools.ObjentryEditor.ViewModels
{
    public class ObjentryViewModel : GenericListModel<ObjentryViewModel.ObjentryEntryViewModel>
    {
        public class ObjentryEntryViewModel : BaseNotifyPropertyChanged
        {
            public Objentry Objentry { get; }

            public ObjentryEntryViewModel(Objentry objEntry)
            {
                Objentry = objEntry;
            }

            public string Name => $"{Id} {ModelName}";

            public string Id => $"{Objentry.ObjectId:X02}";

            public ushort ObjectId
            {
                get => Objentry.ObjectId;
                set { Objentry.ObjectId = value; OnPropertyChanged(nameof(Name)); }
            }

            public ushort Unknown02 { get => Objentry.Unknown02; set => Objentry.Unknown02 = value; }
            public Objentry.Type ObjectType { get => Objentry.ObjectType; set => Objentry.ObjectType = value; }
            public byte Unknown05 { get => Objentry.Unknown05; set => Objentry.Unknown05 = value; }
            public byte Unknown06 { get => Objentry.Unknown06; set => Objentry.Unknown06 = value; }
            public byte WeaponJoint { get => Objentry.WeaponJoint; set => Objentry.WeaponJoint = value; }
            public string ModelName { get => Objentry.ModelName; set => Objentry.ModelName = value; }
            public string AnimationName { get => Objentry.AnimationName; set => Objentry.AnimationName = value; }
            public uint Unknown48 { get => Objentry.Unknown48; set => Objentry.Unknown48 = value; }
            public ushort NeoStatus { get => Objentry.NeoStatus; set => Objentry.NeoStatus = value; }
            public ushort NeoMoveset { get => Objentry.NeoMoveset; set => Objentry.NeoMoveset = value; }
            public uint Unknown50 { get => Objentry.Unknown50; set => Objentry.Unknown50 = value; }
            public byte SpawnLimiter { get => Objentry.SpawnLimiter; set => Objentry.SpawnLimiter = value; }
            public byte Unknown55 { get => Objentry.Unknown55; set => Objentry.Unknown55 = value; }
            public byte Unknown56 { get => Objentry.Unknown56; set => Objentry.Unknown56 = value; }
            public byte Unknown57 { get => Objentry.Unknown57; set => Objentry.Unknown57 = value; }
            public ushort SpawnObject1 { get => Objentry.SpawnObject1; set => Objentry.SpawnObject1 = value; }
            public ushort SpawnObject2 { get => Objentry.SpawnObject2; set => Objentry.SpawnObject2 = value; }
            public ushort SpawnObject3 { get => Objentry.SpawnObject3; set => Objentry.SpawnObject3 = value; }
            public ushort Unknown5e { get => Objentry.Unknown5e; set => Objentry.Unknown5e = value; }

            public override string ToString() => Name;
        }


        private readonly int _type;
        private string _searchTerm;

        public EnumModel<Objentry.Type> ObjEntryTypes { get; }

        public ObjentryViewModel(BaseTable<Objentry> objentry) :
            this(objentry.Id, objentry.Items)
        { }

        public ObjentryViewModel(int type, IEnumerable<Objentry> items) :
            base(items.Select(Map))
        {
            _type = type;
            ObjEntryTypes = new EnumModel<Objentry.Type>();
            AddAndSelectCommand = new RelayCommand(x =>
            {
                AddCommand.Execute(null);
                SelectedIndex = Items.Count - 1;
            });

            CloneCommand = new RelayCommand(x =>
            {
                var clonedItem = Clone(SelectedItem.Objentry);
                Items.Add(new ObjentryEntryViewModel(clonedItem));
                OnPropertyChanged(nameof(Items));
            }, x => SelectedItem != null);
        }

        public RelayCommand AddAndSelectCommand { get; set; }
        public RelayCommand CloneCommand { get; set; }

        public Visibility IsItemEditingVisible => IsItemSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsItemEditMessageVisible => !IsItemSelected ? Visibility.Visible : Visibility.Collapsed;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                PerformFiltering();
            }
        }

        public IEnumerable<Objentry> AsObjEntries() => Items.Select(x => x.Objentry);

        protected override void OnSelectedItem(ObjentryEntryViewModel item)
        {
            base.OnSelectedItem(item);

            OnPropertyChanged(nameof(IsItemEditingVisible));
            OnPropertyChanged(nameof(IsItemEditMessageVisible));
        }

        private void PerformFiltering()
        {
            if (string.IsNullOrWhiteSpace(_searchTerm))
                Filter(FilterNone);
            else
                Filter(FilterByCharacter);
        }

        protected override ObjentryEntryViewModel OnNewItem()
        {
            return new ObjentryEntryViewModel(new Objentry()
            {
                ObjectId = GetObjectIdForNewEntry()
            });
        }

        private Objentry Clone(Objentry source)
        {
            return new Objentry()
            {
                ObjectId = GetObjectIdForNewEntry(),
                Unknown02 = source.Unknown02,
                ObjectType = source.ObjectType,
                Unknown05 = source.Unknown05,
                Unknown06 = source.Unknown06,
                WeaponJoint = source.WeaponJoint,
                ModelName = source.ModelName,
                AnimationName = source.AnimationName,
                Unknown48 = source.Unknown48,
                NeoStatus = source.NeoStatus,
                NeoMoveset = source.NeoMoveset,
                Unknown50 = source.Unknown50,
                SpawnLimiter = source.SpawnLimiter,
                Unknown55 = source.Unknown55,
                Unknown56 = source.Unknown56,
                Unknown57 = source.Unknown57,
                SpawnObject1 = source.SpawnObject1,
                SpawnObject2 = source.SpawnObject2,
                SpawnObject3 = source.SpawnObject3,
                Unknown5e = source.Unknown5e
            };
        }

        private ushort GetObjectIdForNewEntry()
        {
            return (ushort)(Items.LastOrDefault()?.ObjectId + 1 ?? 0);
        }

        private bool FilterNone(ObjentryEntryViewModel arg) => true;

        private bool FilterByCharacter(ObjentryEntryViewModel arg) =>
            arg.Name.ToUpper().Contains(SearchTerm.ToUpper());

        private static ObjentryEntryViewModel Map(Objentry item) =>
            new ObjentryEntryViewModel(item);
    }
}
