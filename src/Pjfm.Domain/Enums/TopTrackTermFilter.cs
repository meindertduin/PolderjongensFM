using System;
using System.Collections.Generic;
using Pjfm.Domain.Enums;

namespace Pjfm.WebClient.Services
{
    public enum TopTrackTermFilter
    {
        ShortTerm = 0,
        ShortMediumTerm = 1,
        MediumTerm = 2,
        MediumLongTerm = 3,
        LongTerm = 4,
        AllTerms = 5,
    }
    
    // Deze staat hier inplaats van in de extensions directory omdat deze dan alleen over deze namespace gaat
    public static class Extensions
    {
        public static List<TopTrackTerm> ConvertToTopTrackTerms(this TopTrackTermFilter value)
        {
            var convertedTerms = new List<TopTrackTerm>();

            switch (value)
            {
                case TopTrackTermFilter.ShortTerm:
                    convertedTerms.Add(TopTrackTerm.ShortTerm);
                    break;
                case TopTrackTermFilter.MediumTerm:
                    convertedTerms.Add(TopTrackTerm.MediumTerm);
                    break;
                case TopTrackTermFilter.LongTerm:
                    convertedTerms.Add(TopTrackTerm.LongTerm);
                    break;
                case TopTrackTermFilter.ShortMediumTerm:
                    convertedTerms.Add(TopTrackTerm.ShortTerm);
                    convertedTerms.Add(TopTrackTerm.MediumTerm);
                    break;
                case TopTrackTermFilter.MediumLongTerm:
                    convertedTerms.Add(TopTrackTerm.MediumTerm);
                    convertedTerms.Add(TopTrackTerm.LongTerm);
                    break;
                case TopTrackTermFilter.AllTerms:
                    convertedTerms.Add(TopTrackTerm.ShortTerm);
                    convertedTerms.Add(TopTrackTerm.MediumTerm);
                    convertedTerms.Add(TopTrackTerm.LongTerm);
                    break;
                default:
                    throw new Exception("Cant convert TopTrackTermFilter, " +
                                        "Reason being that the conversion is probably not implemented");
            }
            
            return convertedTerms;
        }
    }
}