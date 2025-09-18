using System;

namespace CSharpAlgorithms.Interfaces;
public interface IDataDump<T>
{
    void Dump(ReadOnlySpan<T> data);
}