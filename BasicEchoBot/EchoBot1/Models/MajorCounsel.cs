using Bot.Builder.Community.Dialogs.FormFlow;
using System;

namespace EchoBot1.Models
{
    public enum MajorType
    {
        SoftwareEngineer,
        MechanicalEngineer
    }

    [Serializable]
    public class MajorCounsel
    {
        public MajorType? TypeOfMajor { get; set; }
        public static IForm<MajorCounsel> BuildForm()
        {
            return new FormBuilder<MajorCounsel>()
                .Message("nganh hoc")
                .Field(nameof(TypeOfMajor))
                .Confirm("xac nhan", p => false)
                .Build();
        }
    }
}
