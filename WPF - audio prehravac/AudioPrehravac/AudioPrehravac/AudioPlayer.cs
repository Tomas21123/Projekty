using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;

namespace AudioPrehravac
{
    class AudioPlayer : IDisposable
    {
        private readonly string _path;
        private AudioFileReader _audioFile;
        private WaveOutEvent _outputDevice;
        private bool _isPlaying;
        private bool _isManualStop;

        public event Action<float> OnAmplitudeChanged;
        public event Action SongFinished;

        private SampleChannel _sampleChannel;
        private MeteringSampleProvider _meteringProvider;

        public AudioPlayer(string pathToFolderWithSongs)
        {
            _path = pathToFolderWithSongs;
            _outputDevice = new WaveOutEvent();
            _outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
        }

        public void SetSong(string songFileName, bool manualChange = true)
        {
            try
            {
                if (manualChange)
                    _isManualStop = true; // jen když uživatel mění song ručně
                _outputDevice?.Stop();
                _audioFile?.Dispose();

                string fullPath = System.IO.Path.Combine(_path, songFileName);
                if (!System.IO.File.Exists(fullPath))
                    return;

                _audioFile = new AudioFileReader(fullPath);
                _sampleChannel = new SampleChannel(_audioFile, true);
                _meteringProvider = new MeteringSampleProvider(_sampleChannel);
                _meteringProvider.StreamVolume += MeteringProvider_StreamVolume;

                _outputDevice.Init(_meteringProvider);
                _isPlaying = false;
            }
            catch { }
        }


        private void MeteringProvider_StreamVolume(object sender, StreamVolumeEventArgs e)
        {
            if (e.MaxSampleValues != null && e.MaxSampleValues.Length > 0)
            {
                float amplitude = e.MaxSampleValues[0];
                OnAmplitudeChanged?.Invoke(amplitude);
            }
        }

        public void PlayStop()
        {
            if (_audioFile == null || _outputDevice == null)
                return;

            if (!_isPlaying)
            {
                _isManualStop = false; // normální spuštění
                _outputDevice.Play();
                _isPlaying = true;
            }
            else
            {
                _isManualStop = true; // uživatel klikl pauzu
                _outputDevice.Stop();
                _isPlaying = false;
            }
        }

        public void Play()
        {
            if (_audioFile == null) return;

            // reset pozice na začátek pokud jsme na konci
            if (_audioFile.Position >= _audioFile.Length)
                _audioFile.Position = 0;

            _isManualStop = false; // jen normální spuštění
            _outputDevice.Play();
            _isPlaying = true;
        }


        public void SetVolume(int hlasitost)
        {
            if (_audioFile == null) return;
            float volume = Math.Clamp(hlasitost / 100f, 0f, 1f);
            _audioFile.Volume = volume;
        }

        public double GetSongLengthSeconds()
        {
            return _audioFile?.TotalTime.TotalSeconds ?? 0;
        }

        public double CurrentPositionSeconds()
        {
            return _audioFile?.CurrentTime.TotalSeconds ?? 0;
        }

        public void Seek(int seconds)
        {
            if (_audioFile != null)
                _audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        public void Dispose()
        {
            _outputDevice?.Dispose();
            _audioFile?.Dispose();
        }

        private void OutputDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            _isPlaying = false;

            // Spustíme SongFinished pouze pokud to **není manuální zastavení**
            if (!_isManualStop && _audioFile != null && _audioFile.Position >= _audioFile.Length - 1)
            {
                SongFinished?.Invoke();
            }

            // reset flag
            _isManualStop = false;
        }
    }
}
