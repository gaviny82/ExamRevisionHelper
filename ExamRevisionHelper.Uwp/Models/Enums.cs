namespace ExamRevisionHelper.Models
{
    public enum Curriculums { IGCSE, ALevel }
    public enum ExamSeries { Spring, Summer, Winter, Specimen }
    public enum ResourceStates { Online, Downloading, Offline }

    public enum ResourceType
    {
        QuestionPaper,
        Insert,
        MarkScheme,
        ListeningAudio,
        SpeakingTestCards,
        Transcript,
        TeachersNotes,
        ConfidentialInstructions,
        ExaminersReport,
        GradeThreshold,
        Unknown
    }
}
