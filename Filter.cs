// Copyright (C) 2025 Reetus
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ClassicAssist.Data.Autoloot;
using ClassicAssist.Shared.Misc;
using ClassicAssist.UO.Objects;

namespace ClassicAssist.ECVCustomFilterExample
{
    public static class Filter
    {
        public enum ResistCliloc
        {
            Physical = 1060448,
            Fire = 1060447,
            Cold = 1060445,
            Poison = 1060449,
            Energy = 1060446
        }

        public static void Initialize( ObservableCollection<PropertyEntry> constraints )
        {
            AddTotalResistsConstraint( constraints );
            AddOwnerConstraint( constraints );
        }

        private static void AddTotalResistsConstraint( ObservableCollection<PropertyEntry> constraints )
        {
            var existing = constraints.FirstOrDefault( e => e.Name == "Total Resists" );

            if ( existing != null )
            {
                constraints.Remove( existing );
            }

            constraints.AddSorted( new PropertyEntry
            {
                Name = "Total Resists",
                ConstraintType = PropertyType.PredicateWithValue,
                Predicate = ( entity, entry ) =>
                {
                    if ( !( entity is Item item ) )
                    {
                        return false;
                    }

                    var totalResists = GetTotalResists( item );

                    switch ( entry.Operator )
                    {
                        case AutolootOperator.GreaterThan:
                            return totalResists > entry.Value;
                        case AutolootOperator.LessThan:
                            return totalResists < entry.Value;
                        case AutolootOperator.Equal:
                            return totalResists == entry.Value;
                        case AutolootOperator.NotEqual:
                            return totalResists != entry.Value;
                        case AutolootOperator.NotPresent:
                        default:
                            return false;
                    }
                }
            } );
        }

        private static void AddOwnerConstraint( IList<PropertyEntry> constraints )
        {
            var existing = constraints.FirstOrDefault( e => e.Name == "Owner" );

            if ( existing != null )
            {
                constraints.Remove( existing );
            }

            constraints.AddSorted( new PropertyEntry
            {
                Name = "Owner",
                ConstraintType = PropertyType.PredicateWithValue,
                Predicate = ( entity, entry ) =>
                {
                    if ( !( entity is Item item ) )
                    {
                        return false;
                    }

                    var owner = item.Owner;

                    switch ( entry.Operator )
                    {
                        case AutolootOperator.GreaterThan:
                            return owner > entry.Value;
                        case AutolootOperator.LessThan:
                            return owner < entry.Value;
                        case AutolootOperator.Equal:
                            return owner == entry.Value;
                        case AutolootOperator.NotEqual:
                            return owner != entry.Value;
                        case AutolootOperator.NotPresent:
                        default:
                            return false;
                    }
                }
            } );
        }

        private static int GetTotalResists( Entity item )
        {
            var values = Enum.GetValues( typeof( ResistCliloc ) );

            return ( from int cliloc in values
                where item.Properties != null && item.Properties.Any( e => e.Cliloc == cliloc )
                select item.Properties.First( e => e.Cliloc == cliloc )
                into property
                select Convert.ToInt32( property.Arguments[0] ) ).Sum();
        }
    }
}