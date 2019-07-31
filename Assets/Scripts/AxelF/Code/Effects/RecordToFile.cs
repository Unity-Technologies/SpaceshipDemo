
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Buffer = System.Buffer;

namespace AxelF {

public sealed class RecordToFile : MonoBehaviour {
    object _locker = 0;
    Queue<byte[]> _freed = new Queue<byte[]>(8);
    Queue<byte[]> _inuse = new Queue<byte[]>(8);
    List<byte[]> _proc = new List<byte[]>(8);
    BinaryWriter _writer;
    bool _recording;

    void OnDestroy() {
        StopRecording();
    }

    public void StartRecording(string path) {
        StopRecording();
        if (!path.EndsWith(".wav"))
            path += ".wav";
        lock (_locker) {
            _writer = new BinaryWriter(new FileStream(path, FileMode.Create));
            for (int i = 0; i < 44; ++i)
                _writer.Write((byte) 0x00);
            _recording = true;
        }
    }

    public int StopRecording() {
        int len = 0;
        if (_recording)
            lock (_locker) {
                len = (int) _writer.BaseStream.Length;
                _recording = false;

                short format = 3;
                short channels = 2;
                int sampleRate = AudioSettings.outputSampleRate;
                int byteRate = sampleRate * sizeof(float) * channels;
                short blockAlign = (short) (sizeof(float) * channels);
                short bitsPerSample = (short) (sizeof(float) * 8);
                _writer.Seek(0, SeekOrigin.Begin);
                _writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                _writer.Write((int) (_writer.BaseStream.Length - 8));
                _writer.Write(Encoding.ASCII.GetBytes("WAVE"));
                _writer.Write(Encoding.ASCII.GetBytes("fmt "));
                _writer.Write((int) 16);
                _writer.Write(format);
                _writer.Write(channels);
                _writer.Write(sampleRate);
                _writer.Write(byteRate);
                _writer.Write(blockAlign);
                _writer.Write(bitsPerSample);
                _writer.Write(Encoding.ASCII.GetBytes("data"));
                _writer.Write((int) (_writer.BaseStream.Length - 44));
                _writer.Close();
            }
        return len;
    }

    void Update() {
        if (_recording) {
            lock (_locker)
                while (_inuse.Count > 0)
                    _proc.Add(_inuse.Dequeue());
            foreach (var i in _proc)
                _writer.Write(i);
            lock (_locker)
                foreach (var i in _proc)
                    _freed.Enqueue(i);
            _proc.Clear();
        }
    }

    void OnAudioFilterRead(float[] data, int channels) {
        lock (_locker)
            if (_recording) {
                var buf = _freed.Count > 0 ? _freed.Dequeue() : null;
                if (buf == null || buf.Length != data.Length * sizeof(float))
                    buf = new byte[data.Length * sizeof(float)];
                Buffer.BlockCopy(data, 0, buf, 0, buf.Length);
                _inuse.Enqueue(buf);
            }
    }
}

} // AxelF

