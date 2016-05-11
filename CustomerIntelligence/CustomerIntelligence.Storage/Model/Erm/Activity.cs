namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Letter : ActivityBase { }

    public sealed class Task : ActivityBase { }

    public sealed class Phonecall : ActivityBase { }

    public sealed class Appointment : ActivityBase { }

    public sealed class LetterReference : ActivityReference { }

    public sealed class TaskReference : ActivityReference { }

    public sealed class PhonecallReference : ActivityReference { }

    public sealed class AppointmentReference : ActivityReference { }
}