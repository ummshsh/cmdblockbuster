﻿using BlockBuster.Field;
using BlockBuster.Tetrominoes;

namespace BlockBuster.Settings
{
    internal class DebugPlayfields
    {
        internal static InnerPlayfield GetPlayfield(string name)
        {
            var fieldToReturn = new InnerPlayfield(10, 22);

            var debugField = GetField(name);

            for (int row = 0; row < 22; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < 10; rowItemIndex++)
                {
                    fieldToReturn[row, rowItemIndex] = debugField[row, rowItemIndex] > 0 ? TetrominoCellType.Blue : TetrominoCellType.Empty;
                }
            }

            return fieldToReturn;
        }

        private static int[,] GetField(string name)
        {
            return name switch
            {
                "tetris" => new int[,]
                {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,0},
                },
                "tspinsingle" => new int[,]
                {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,1,0,0},
                {0,0,0,1,0,0,0,1,1,0},
                {1,1,1,1,1,0,1,1,1,1},
                },
                "minitspinsingle" => new int[,]
                {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1},
                {0,0,0,0,1,1,0,0,1,1},
                {1,1,1,1,1,0,0,0,1,1},
                {1,1,1,1,1,1,1,1,1,0},
                },
                "tspindouble" => new int[,]
                {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,1},
                {0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,0,1},
                {1,1,1,1,1,1,1,0,0,1},
                {1,1,1,1,1,1,1,0,0,1},
                {1,1,1,1,1,1,1,0,0,1},
                },
                _ => throw new System.Exception("Debug playfield state was not found"),
            };
        }
    }
}