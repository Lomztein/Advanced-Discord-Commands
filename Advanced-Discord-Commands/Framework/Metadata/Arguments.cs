﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lomztein.AdvDiscordCommands.Framework
{
    public class Arguments : IEnumerable<object[]>
    {
        public const char SEPERATOR = ',';

        public readonly string Raw;
        private readonly object[][] _arguments;

        public Arguments (string raw, params object[][] arguments)
        {
            Raw = raw;
            _arguments = arguments;
        }

        public Arguments(string raw, List<List<object>> arguments)
        {
            Raw = raw;
            _arguments = arguments.Select(x => x.ToArray()).ToArray();
        }

        public object[] this[int index] {
            get => _arguments[index];
        }

        public object this[int x, int y] {
            get => _arguments[x][y];
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            return (_arguments as IEnumerable<object[]>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _arguments.GetEnumerator();
        }
    }
}
