using System;
using System.IO;
using System.Text;

namespace CSharpAlgorithms.Data;

public static class DataValidation
{

    public static bool IsValidWavFile(string filePath, out string reason)
    {
        reason = string.Empty;

        if (!File.Exists(filePath))
        {
            reason = "File does not exist.";
            return false;
        }

        try
        {
            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fs);

            if (fs.Length < 44)
            {
                reason = "File is too small to be a valid WAV file (must be at least 44 bytes).";
                return false;
            }

            // --- RIFF header ---
            string riff = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (riff != "RIFF")
            {
                reason = $"Invalid header: expected 'RIFF', found '{riff}'.";
                return false;
            }

            // File size field (next 4 bytes, skip)
            reader.ReadUInt32();

            // --- WAVE header ---
            string wave = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (wave != "WAVE")
            {
                reason = $"Invalid format: expected 'WAVE', found '{wave}'.";
                return false;
            }

            // --- fmt chunk ---
            string fmt = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (fmt != "fmt ")
            {
                reason = $"Missing 'fmt ' chunk, found '{fmt}'.";
                return false;
            }

            uint fmtChunkSize = reader.ReadUInt32();
            if (fmtChunkSize < 16)
            {
                reason = $"Invalid fmt chunk size ({fmtChunkSize}), must be at least 16.";
                return false;
            }

            ushort audioFormat = reader.ReadUInt16();
            if (audioFormat != 1)
            {
                reason = $"Unsupported audio format code ({audioFormat}). Only PCM (1) is supported.";
                return false;
            }

            ushort numChannels = reader.ReadUInt16();
            uint sampleRate = reader.ReadUInt32();
            uint byteRate = reader.ReadUInt32();
            ushort blockAlign = reader.ReadUInt16();
            ushort bitsPerSample = reader.ReadUInt16();

            // --- Sanity checks ---
            if (numChannels == 0 || numChannels > 8)
            {
                reason = $"Invalid channel count: {numChannels}.";
                return false;
            }

            if (sampleRate < 8000 || sampleRate > 192000)
            {
                reason = $"Unusual sample rate: {sampleRate}.";
                return false;
            }

            if (bitsPerSample != 8 && bitsPerSample != 16 && bitsPerSample != 24 && bitsPerSample != 32)
            {
                reason = $"Invalid bits per sample: {bitsPerSample}.";
                return false;
            }

            // --- Search for data chunk ---
            while (reader.BaseStream.Position < reader.BaseStream.Length - 8)
            {
                string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                uint chunkSize = reader.ReadUInt32();

                if (chunkId == "data")
                {
                    if (chunkSize == 0)
                    {
                        reason = "Data chunk has zero length.";
                        return false;
                    }

                    if (reader.BaseStream.Position + chunkSize > reader.BaseStream.Length)
                    {
                        reason = "Data chunk size exceeds file length.";
                        return false;
                    }

                    // All checks passed
                    return true;
                }

                // Skip unrecognized chunks
                reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
            }

            reason = "Missing 'data' chunk.";
            return false;
        }
        catch (Exception ex)
        {
            reason = $"Error reading file: {ex.Message}";
            return false;
        }
    }
}