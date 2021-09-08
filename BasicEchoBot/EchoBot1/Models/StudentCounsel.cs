using Bot.Builder.Community.Dialogs.FormFlow;
using System;

namespace EchoBot1.Models
{
    [Serializable]
    public class StudentCounsel
    {
        [Describe(Description = "Tu van", Image ="")]
        [Prompt("ban can tu van ve gi a!")]
        public CounselType? TypeOfCounsel { get; set; }
        public static IForm<StudentCounsel> BuildForm()
        {
            return new FormBuilder<StudentCounsel>()
                .Field(nameof(TypeOfCounsel))
                .Confirm("xan nhan", p => false)
                .Build();
        }
    }
}
