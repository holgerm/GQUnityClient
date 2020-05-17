using System;
using System.Collections.Generic;
using System.Text;

public static class StringExtensions
{
    public static List<string> SplitWithMaskedSeparator(this string original, char separator = ',')
    {
        var result = new List<string>();
        StringBuilder part = null;

        for (var i = 0; i < original.Length; i++)
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (part == null)
                part = new StringBuilder();

            if (original[i] != separator)
            {
                part.Append(original[i]);
                
                if (original.Length > i + 1)
                {
                    // keep collecting for current part:
                    continue;
                }
                else
                {
                    // this is the last char of the last part:
                    result.Add(part.ToString());
                    break;
                }
            }

            else
                // we found a separator:
            {
                if (original.Length > i + 1)
                {
                    if (original[i + 1] == separator)
                    {
                        // separator is masked, add it as normal char to the part and skip the mask:
                        part.Append(original[i]);
                        i++;
                        if (original.Length == i + 1)
                        {
                            // if the second (masking) separator is the last char we finalize the part:
                            result.Add(part.ToString());
                        }

                        continue;
                    }
                    else
                    {
                        // it really is a valid separator, finalize part and start collecting anew:
                        if (part.Length > 0)
                            result.Add(part.ToString());
                        part = new StringBuilder();
                        continue;
                    }
                }
                else
                {
                    // this separator is the last char in the whole string. finalize the last part:
                    if (part.Length > 0)
                        result.Add(part.ToString());
                    break;
                }
            }
        }

        return result;
    }
}