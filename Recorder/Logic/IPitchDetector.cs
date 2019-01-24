namespace Recorder.Logic
{
    public interface IPitchDetector
    {
         float DetectPitch(float[] buffer, int frames);
    }
}