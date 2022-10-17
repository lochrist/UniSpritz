using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UniMini
{
    public interface Font
    {
        void DrawText(Layer layer, string text, int x, int y, Color color);
        Vector2Int GetTextSize(string text);
    }

    class BitmapFont : Font
    {
        public static Dictionary<char, byte[,]> glyphs;
        public int spaceBetweenLetter = 1;

        public void DrawText(Layer layer, string text, int x, int y, Color color)
        {
            var xOrig = x;
            foreach (char l in text)
            {
                if (l == '\n')
                {
                    y += 6;
                    x = xOrig;
                    continue;
                }

                if (glyphs.ContainsKey(l))
                {
                    var glyph = glyphs[l];
                    for (var i = 0; i < glyph.GetLength(0); i++)
                    {
                        for (var j = 0; j < glyph.GetLength(1); j++)
                        {
                            if (glyph[i, j] == 1)
                            {
                                layer.DrawPixel(x + j, y + i, color);
                            }
                        }
                    }

                    x += glyph.GetLength(1) + spaceBetweenLetter;
                }
            }
        }

        public Vector2Int GetTextSize(string text)
        {
            var size = new Vector2Int(0, 0);
            for(var i = 0; i < text.Length; ++i)
            {
                var c = text[i];
                if (glyphs.ContainsKey(c))
                {
                    var g = glyphs[c];
                    size.x += g.GetLength(1);
                    if (i < text.Length - 1)
                    {
                        size.x += spaceBetweenLetter;
                    }
                    if (g.GetLength(0) > size.y)
                        size.y = g.GetLength(0);
                }
            }
            size.y += spaceBetweenLetter;
            return size;
        }

    }

    class DefaultFont : BitmapFont
    {
        public static byte[,] f_empty = {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_exclamation = {
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 0, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_quotes = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_hashtag = {
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_dolar = {
            { 1, 1, 1 },
            { 1, 1, 0 },
            { 0, 1, 1 },
            { 1, 1, 1 },
            { 0, 1, 0 },
        };

        public static byte[,] f_percentage = {
            { 1, 0, 1 },
            { 0, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 1, 0, 1 },
        };

        public static byte[,] f_ampersand = {
            { 1, 1, 0 },
            { 1, 1, 0 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_tone = {
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_par_open = {
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_par_close = {
            { 0, 1, 0 },
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 0, 1, 0 },
        };

        public static byte[,] f_astherisc = {
            { 1, 0, 1 },
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 1, 0, 1 },
        };

        public static byte[,] f_plus = {
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_comma = {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_dash = {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_dot = {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_slash = {
            { 0, 0, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_0 = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_1 = {
            { 1, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_2 = {
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_3 = {
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 0, 1, 1 },
            { 0, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_4 = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
        };

        public static byte[,] f_5 = {
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_6 = {
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_7 = {
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
        };

        public static byte[,] f_8 = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_9 = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
        };

        public static byte[,] f_colon = {
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_semicolon = {
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_less = {
            { 0, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
        };

        public static byte[,] f_equals = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 },
        };

        public static byte[,] f_greater = {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_question = {
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 0, 1, 1 },
            { 0, 0, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_at = {
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 0 },
            { 0, 1, 1 },
        };

        public static byte[,] f_a = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_b = {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_c = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_d = {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
        };

        public static byte[,] f_e = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 1, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_f = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 1, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_g = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_h = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_i = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_j = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 0 },
        };

        public static byte[,] f_k = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_l = {
            { 0, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_m = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_n = {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_o = {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
        };

        public static byte[,] f_p = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 0 },
        };

        public static byte[,] f_q = {
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 0, 1, 1 },
        };

        public static byte[,] f_r = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 1, 0, 1 },
        };

        public static byte[,] f_s = {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 1, 0, 0 },
            { 0, 0, 1 },
            { 1, 1, 0 },
        };

        public static byte[,] f_t = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_u = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 0, 1, 1 },
        };

        public static byte[,] f_v = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 0, 1, 0 },
        };

        public static byte[,] f_w = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_x = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_y = {
            { 0, 0, 0 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_z = {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_bracket_open = {
            { 1, 1, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 1, 0 },
        };

        public static byte[,] f_backslash = {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
        };

        public static byte[,] f_bracket_close = {
            { 0, 1, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 0, 0, 1 },
            { 0, 1, 1 },
        };

        public static byte[,] f_carat = {
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_underscore = {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_back_quote = {
            { 0, 1, 0 },
            { 0, 0, 1 },
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_A = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_B = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_C = {
            { 0, 1, 1 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 0, 1, 1 },
        };

        public static byte[,] f_D = {
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_E = {
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 1, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_F = {
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 1, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_G = {
            { 0, 1, 1 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_H = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_I = {
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_J = {
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 0 },
        };

        public static byte[,] f_K = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_L = {
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_M = {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_N = {
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_O = {
            { 0, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
        };

        public static byte[,] f_P = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 0, 0 },
        };

        public static byte[,] f_Q = {
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 0, 1, 1 },
        };

        public static byte[,] f_R = {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_S = {
            { 0, 1, 1 },
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 1, 0 },
        };

        public static byte[,] f_T = {
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_U = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 0, 1, 1 },
        };

        public static byte[,] f_V = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 0, 1, 0 },
        };

        public static byte[,] f_W = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_X = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
        };

        public static byte[,] f_Y = {
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_Z = {
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 1, 1, 1 },
        };

        public static byte[,] f_brace_open = {
            { 0, 1, 1 },
            { 0, 1, 0 },
            { 1, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 1 },
        };

        public static byte[,] f_pipe = {
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 0 },
        };

        public static byte[,] f_brace_close = {
            { 1, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 1 },
            { 0, 1, 0 },
            { 1, 1, 0 },
        };

        public static byte[,] f_tilde = {
            { 0, 0, 0 },
            { 0, 0, 1 },
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 0, 0, 0 },
        };

        public static byte[,] f_nubbin = {
            { 0, 0, 0 },
            { 0, 1, 0 },
            { 1, 0, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 },
        };

        public static byte[,] f_block = {
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
        };

        public static byte[,] f_semi_block = {
            { 1, 0, 1, 0, 1, 0, 1 },
            { 0, 1, 0, 1, 0, 1, 0 },
            { 1, 0, 1, 0, 1, 0, 1 },
            { 0, 1, 0, 1, 0, 1, 0 },
            { 1, 0, 1, 0, 1, 0, 1 },
        };

        public static byte[,] f_alien = {
            { 1, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 1, 1, 0, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_downbutton = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_quasi_block = {
            { 1, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 1, 0, 0 },
        };

        public static byte[,] f_shuriken = {
            { 0, 0, 1, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0 },
        };

        public static byte[,] f_shiny_ball = {
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 0, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
        };

        public static byte[,] f_heart = {
            { 0, 1, 1, 0, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0 },
        };

        public static byte[,] f_sauron = {
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 0, 1, 1, 0 },
            { 1, 1, 1, 0, 1, 1, 1 },
            { 0, 1, 1, 0, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
        };

        public static byte[,] f_human = {
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 1, 0, 1, 0, 0 },
        };

        public static byte[,] f_house = {
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 0, 1, 0, 1, 0, 1, 0 },
            { 0, 1, 0, 1, 1, 1, 0 },
        };

        public static byte[,] f_leftbutton = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 1, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 1, 0, 0, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_face = {
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1 },
        };

        public static byte[,] f_note = {
            { 0, 0, 0, 1, 1, 1, 0 },
            { 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0 },
            { 0, 1, 1, 1, 0, 0, 0 },
            { 0, 1, 1, 1, 0, 0, 0 },
        };

        public static byte[,] f_obutton = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 1, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_diamond = {
            { 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0 },
        };

        public static byte[,] f_dot_line = {
            { 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 1, 0, 1, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0 },
        };
        public static byte[,] f_rightbutton = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 0, 0, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };
        public static byte[,] f_star = {
            { 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 0, 0, 0, 1, 0 },
        };

        public static byte[,] f_hourclass = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_upbutton = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 1, 0, 1, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_down_arrows = {
            { 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 1, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 1, 0, 1 },
            { 0, 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0 },
        };

        public static byte[,] f_triangle_wave = {
            { 0, 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 1, 0, 0 },
            { 0, 1, 0, 1, 0, 1, 0 },
            { 0, 0, 1, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0, 0 },
        };

        public static byte[,] f_xbutton = {
            { 0, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 0, 1, 0, 1, 1 },
            { 1, 1, 1, 0, 1, 1, 1 },
            { 1, 1, 0, 1, 0, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 0 },
        };

        public static byte[,] f_horizontal_lines = {
            { 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1 },
        };

        public static byte[,] f_vertical_lines = {
            { 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 0, 1 },
        };

        public DefaultFont()
        {
            glyphs = new Dictionary<char, byte[,]>();
            glyphs.Add(' ', f_empty);
            glyphs.Add('!', f_exclamation);
            glyphs.Add('"', f_quotes);
            glyphs.Add('#', f_hashtag);
            glyphs.Add('$', f_dolar);
            glyphs.Add('%', f_percentage);
            glyphs.Add('&', f_ampersand);
            glyphs.Add('\'', f_tone);
            glyphs.Add('(', f_par_open);
            glyphs.Add(')', f_par_close);
            glyphs.Add('*', f_astherisc);
            glyphs.Add('+', f_plus);
            glyphs.Add(',', f_comma);
            glyphs.Add('-', f_dash);
            glyphs.Add('.', f_dot);
            glyphs.Add('/', f_slash);
            glyphs.Add('0', f_0);
            glyphs.Add('1', f_1);
            glyphs.Add('2', f_2);
            glyphs.Add('3', f_3);
            glyphs.Add('4', f_4);
            glyphs.Add('5', f_5);
            glyphs.Add('6', f_6);
            glyphs.Add('7', f_7);
            glyphs.Add('8', f_8);
            glyphs.Add('9', f_9);
            glyphs.Add(':', f_colon);
            glyphs.Add(';', f_semicolon);
            glyphs.Add('<', f_less);
            glyphs.Add('=', f_equals);
            glyphs.Add('>', f_greater);
            glyphs.Add('?', f_question);
            glyphs.Add('@', f_at);
            glyphs.Add('a', f_a);
            glyphs.Add('b', f_b);
            glyphs.Add('c', f_c);
            glyphs.Add('d', f_d);
            glyphs.Add('e', f_e);
            glyphs.Add('f', f_f);
            glyphs.Add('g', f_g);
            glyphs.Add('h', f_h);
            glyphs.Add('i', f_i);
            glyphs.Add('j', f_j);
            glyphs.Add('k', f_k);
            glyphs.Add('l', f_l);
            glyphs.Add('m', f_m);
            glyphs.Add('n', f_n);
            glyphs.Add('o', f_o);
            glyphs.Add('p', f_p);
            glyphs.Add('q', f_q);
            glyphs.Add('r', f_r);
            glyphs.Add('s', f_s);
            glyphs.Add('t', f_t);
            glyphs.Add('u', f_u);
            glyphs.Add('v', f_v);
            glyphs.Add('w', f_w);
            glyphs.Add('x', f_x);
            glyphs.Add('y', f_y);
            glyphs.Add('z', f_z);
            glyphs.Add('[', f_bracket_open);
            glyphs.Add('\\', f_backslash);
            glyphs.Add(']', f_bracket_close);
            glyphs.Add('^', f_carat);
            glyphs.Add('_', f_underscore);
            glyphs.Add('`', f_back_quote);
            glyphs.Add('A', f_A);
            glyphs.Add('B', f_B);
            glyphs.Add('C', f_C);
            glyphs.Add('D', f_D);
            glyphs.Add('E', f_E);
            glyphs.Add('F', f_F);
            glyphs.Add('G', f_G);
            glyphs.Add('H', f_H);
            glyphs.Add('I', f_I);
            glyphs.Add('J', f_J);
            glyphs.Add('K', f_K);
            glyphs.Add('L', f_L);
            glyphs.Add('M', f_M);
            glyphs.Add('N', f_N);
            glyphs.Add('O', f_O);
            glyphs.Add('P', f_P);
            glyphs.Add('Q', f_Q);
            glyphs.Add('R', f_R);
            glyphs.Add('S', f_S);
            glyphs.Add('T', f_T);
            glyphs.Add('U', f_U);
            glyphs.Add('V', f_V);
            glyphs.Add('W', f_W);
            glyphs.Add('X', f_X);
            glyphs.Add('Y', f_Y);
            glyphs.Add('Z', f_Z);
            glyphs.Add('{', f_brace_open);
            glyphs.Add('|', f_pipe);
            glyphs.Add('}', f_brace_close);
            glyphs.Add('~', f_tilde);
            glyphs.Add((char)127, f_nubbin);
            glyphs.Add((char)128, f_block);
            glyphs.Add((char)129, f_semi_block);
            glyphs.Add((char)130, f_alien);
            glyphs.Add((char)131, f_downbutton);
            glyphs.Add((char)132, f_quasi_block);
            glyphs.Add((char)133, f_shuriken);
            glyphs.Add((char)134, f_shiny_ball);
            glyphs.Add((char)135, f_heart);
            glyphs.Add((char)136, f_sauron);
            glyphs.Add((char)137, f_human);
            glyphs.Add((char)138, f_house);
            glyphs.Add((char)139, f_leftbutton);
            glyphs.Add((char)140, f_face);
            glyphs.Add((char)141, f_note);
            glyphs.Add((char)142, f_obutton);
            glyphs.Add((char)143, f_diamond);
            glyphs.Add((char)144, f_dot_line);
            glyphs.Add((char)145, f_rightbutton);
            glyphs.Add((char)146, f_star);
            glyphs.Add((char)147, f_hourclass);
            glyphs.Add((char)148, f_upbutton);
            glyphs.Add((char)149, f_down_arrows);
            glyphs.Add((char)150, f_triangle_wave);
            glyphs.Add((char)151, f_xbutton);
            glyphs.Add((char)152, f_horizontal_lines);
            glyphs.Add((char)153, f_vertical_lines);
        }
    }

    public class SpriteFont : Font
    {
        public int height = 7;
        public int width = 7;
        public static Dictionary<char, SpriteId> glyphs;

        public SpriteFont(int width, int height)
        {
            this.width = width;
            this.height = height;
            glyphs = new Dictionary<char, SpriteId>();
        }

        public Vector2Int GetTextSize(string text)
        {
            return new Vector2Int(text.Length * width, height);
        }

        public void Add(char start, SpriteId id)
        {
            glyphs[start] = id;
        }

        public void Add(char start, char end, SpriteId[] sprites, int spriteStartIndex = 0)
        {
            var count = end - start;
            if (sprites.Length - spriteStartIndex < count)
            {
                count = sprites.Length - spriteStartIndex;
            }
            for (char i = (char)0; i < count; ++i)
            {
                glyphs[(char)(start + i)] = sprites[spriteStartIndex + i];
            }
        }

        public void DrawText(Layer layer, string text, int x, int y, Color color)
        {
            var xOrig = x;
            foreach (char l in text)
            {
                if (l == '\n')
                {
                    y += 6;
                    x = xOrig;
                    continue;
                }

                // TO check: DrawSprite doesn't care about color.

                if (glyphs.ContainsKey(l))
                {
                    var id = glyphs[l];
                    layer.DrawSprite(id, x, y);
                    x += width + 1;
                }
                else if (l == ' ')
                {
                    x += width + 1;
                }
            }
        }
    }
}