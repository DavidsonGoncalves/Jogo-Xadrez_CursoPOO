﻿using System;
using tabuleiro;
using Xadrez;

namespace Jogo_Xadrez_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Partida_De_Xadrez Partida = new Partida_De_Xadrez();

                while (!Partida.Terminada)
                {
                    try
                    {
                        Console.Clear();
                        Tela.ImprimirPartida(Partida);

                        Console.WriteLine();
                        Console.Write("Origem: ");
                        Posicao origem = Tela.LerPosicao_Xadrez().ToPosicao();
                        Partida.Valida_Pos_Origem(origem);

                        bool[,] PosicoesPossiveis = Partida.Tab.peca(origem).MovimentosPossiveis();

                        Console.Clear();
                        Tela.Imprimir_Tabuleiro(Partida.Tab, PosicoesPossiveis);

                        Console.WriteLine();
                        Console.Write("Destino: ");
                        Posicao destino = Tela.LerPosicao_Xadrez().ToPosicao();
                        Partida.Valida_Pos_Destino(origem, destino);

                        Partida.Jogada(origem, destino);
                    }
                    catch(Tabuleiro_Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                    }
                }


                Console.Clear();
                Tela.ImprimirPartida(Partida);
                


            }
            catch (Tabuleiro_Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
