﻿using OpenKh.Kh2;
using OpenKh.Kh2.Battle;
using OpenKh.Tools.Kh2BattleEditor.Extensions;
using OpenKh.Tools.Kh2BattleEditor.Interfaces;
using OpenKh.Tools.Kh2BattleEditor.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xe.Tools.Wpf.Models;

namespace OpenKh.Tools.Kh2BattleEditor.ViewModels
{
    public class FmlvViewModel : GenericListModel<FmlvFormViewModel>, IBattleGetChanges
    {
        private const int DefaultType = 2;
        private const string entryName = "fmlv";
        
        
        private readonly int _type;
        //private static bool isFinalMix = false;
        public string EntryName => entryName;
        
        public FmlvViewModel() :
            this(new BaseBattle<Fmlv>
            {
                Id = DefaultType,
                Items = new List<Fmlv>()
            })
        {}

        public FmlvViewModel(IEnumerable<Bar.Entry> entries) :
            this(Fmlv.Read(entries.GetBattleStream(entryName)))
        {}


        public FmlvViewModel(BaseBattle<Fmlv> fmlv) :
            this(fmlv.Id, fmlv.Items, (fmlv.Items.Count == 0x2D))
        {}

        public FmlvViewModel(int type, IEnumerable<Fmlv> list, bool isFinalMix) :
            base(list.GroupBy(x => x.FormId).Select(x => new FmlvFormViewModel(x, isFinalMix)))
        {
            _type = type;
        }

        public Stream CreateStream()
        {
            var stream = new MemoryStream();
            new BaseBattle<Fmlv>
            {
                Id = _type,
                Items = Items.SelectMany(form => form).Select(x => x.Fmlv).ToList()

            }.Write(stream);

            return stream;
        }
    }

    public class FmlvFormViewModel : GenericListModel<FmlvFormViewModel.FmlvEntryViewModel>
    {
        private readonly IGrouping<int, Fmlv> fmlvGroup;
        private readonly bool isFinalMix;

        public FmlvFormViewModel(IGrouping<int, Fmlv> x, bool isFinalMix) :
            base(x.Select(y => new FmlvEntryViewModel(y)))
            
        {
            fmlvGroup = x;
            this.isFinalMix = isFinalMix;
        }

        public string Name => FormNameProvider.GetFormName(fmlvGroup.Key, isFinalMix);

        public class FmlvEntryViewModel
        {
            public Fmlv Fmlv { get; }
            public FmlvEntryViewModel(Fmlv fmlv)
            {
                Fmlv = fmlv;
            }

            public string Name => $"{Fmlv.FormLevel}";
            public byte LevelOfMovementAbility { get => Fmlv.LevelOfMovementAbility; set => Fmlv.LevelOfMovementAbility = value; }
            public short AbilityId { get => Fmlv.AbilityId; set => Fmlv.AbilityId = value; }
            public int Exp { get => Fmlv.Exp; set => Fmlv.Exp = value; }

            public override string ToString() => Name;
        }
    }
}