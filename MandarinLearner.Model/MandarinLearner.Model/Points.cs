namespace MandarinLearner.Model
{
    public sealed class Points
    {
        public int Correct { get; private set; }

        public int Incorrect { get; private set; }

        public string Percentage
        {
            get { return GetAveragePercentCorrect() + "%"; }
        }

        private int Total
        {
            get { return Correct + Incorrect; }
        }

        public void AnsweredCorrect()
        {
            Correct++;
        }

        public void AnseredIncorrect()
        {
            Incorrect++;
        }


        private double GetAveragePercentCorrect()
        {
            // Guard against dividing by 0.
            if (Total == 0)
            {
                return 100;
            }

            return (double) Correct*100/((Total));
        }
    }
}