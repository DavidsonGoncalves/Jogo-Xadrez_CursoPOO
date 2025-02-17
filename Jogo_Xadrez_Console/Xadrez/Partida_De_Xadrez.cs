﻿using System.Collections.Generic;
using tabuleiro;

namespace Xadrez
{
    class Partida_De_Xadrez
    {

        public Tabuleiro Tab { get; private set; }
        public int Turno { get; private set; }
        public Cor_Pecas Jogador_Atual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }
        public Peca Vulneravel_EnPassant { get; private set; }



        public Partida_De_Xadrez()
        {
            Tab = new Tabuleiro(8, 8);
            Turno = 1;
            Jogador_Atual = Cor_Pecas.Branca;
            Terminada = false;
            pecas = new HashSet<Peca>();
            Vulneravel_EnPassant = null;
            capturadas = new HashSet<Peca>();
            Colocar_Pecas();

        }

        public Peca Movimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.Retirar_Peca(origem);
            p.Adicionar_Movimentos();
            Peca Peca_Capturada = Tab.Retirar_Peca(destino);
            Tab.ColocarPeca(p, destino);

            if (Peca_Capturada != null)
            {
                capturadas.Add(Peca_Capturada);
            }

            //#jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao OrigemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao DestinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = Tab.Retirar_Peca(OrigemT);
                T.Adicionar_Movimentos();
                Tab.ColocarPeca(T, DestinoT);
            }

            //#jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao OrigemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao DestinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = Tab.Retirar_Peca(OrigemT);
                T.Adicionar_Movimentos();
                Tab.ColocarPeca(T, DestinoT);
            }

            //#jogadaespecial En Passant

            if (p is Peao)
            {
                if (origem.coluna != destino.coluna && Peca_Capturada == null)
                {
                    Posicao posP;
                    if (p.cor == Cor_Pecas.Branca)
                    {
                        posP = new Posicao(destino.linha + 1, destino.coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    Peca_Capturada = Tab.Retirar_Peca(posP);
                    capturadas.Add(Peca_Capturada);
                }
            }

            return Peca_Capturada;
        }


        public void Jogada(Posicao origem, Posicao destino)
        {
            Peca Peca_Capturada = Movimento(origem, destino);

            if (Esta_Em_Xeque(Jogador_Atual))
            {
                Desfaz_Movimento(origem, destino, Peca_Capturada);
                throw new Tabuleiro_Exception("Você não pode se colocar em xeque");
            }

            Peca p = Tab.peca(destino);

            //#jogadaEspecial Promocao
            if (p is Peao)
            {
                if (p.cor == Cor_Pecas.Branca && destino.linha==0 || p.cor == Cor_Pecas.Preta && destino.linha==7)
                {
                    p = Tab.Retirar_Peca(destino);
                    pecas.Remove(p);
                    Peca dama = new Dama(p.cor, Tab);
                    Tab.ColocarPeca(dama, destino);
                    pecas.Add(dama);
                }
            }

            if (Esta_Em_Xeque(adversaria(Jogador_Atual)))
            {
                xeque = true;
            }
            else xeque = false;

            if (TesteXequemate(adversaria(Jogador_Atual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                MudaJogador();
            }

            
            //#jogadaespecial En Passant

            if (p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2))
            {
                Vulneravel_EnPassant = p;
            }
            else
            {
                Vulneravel_EnPassant = null;
            }

        }

        public void Desfaz_Movimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tab.Retirar_Peca(destino);
            p.Diminuir_Movimentos();

            if (pecaCapturada != null)
            {
                Tab.ColocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            Tab.ColocarPeca(p, origem);

            //#jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2)
            {
                Posicao OrigemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao DestinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = Tab.Retirar_Peca(DestinoT);
                T.Diminuir_Movimentos();

                Tab.ColocarPeca(T, origem);
            }

            //#jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2)
            {
                Posicao OrigemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao DestinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = Tab.Retirar_Peca(DestinoT);
                T.Diminuir_Movimentos();
                Tab.ColocarPeca(T, OrigemT);
            }

            //#jogadaespecial En Passant
            if (origem.coluna != destino.coluna && pecaCapturada == Vulneravel_EnPassant)
            {
                Peca peao = Tab.Retirar_Peca(destino);
                Posicao posP;
                if (p.cor == Cor_Pecas.Branca)
                {
                    if (p.cor == Cor_Pecas.Branca)
                    {
                        posP = new Posicao(3, destino.coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.coluna);
                    }
                    Tab.ColocarPeca(peao, posP);
                }
            }

        }

        private void MudaJogador()
        {
            if (Jogador_Atual == Cor_Pecas.Branca)
            {
                Jogador_Atual = Cor_Pecas.Preta;
            }
            else
            {
                Jogador_Atual = Cor_Pecas.Branca;
            }
        }

        public void Valida_Pos_Origem(Posicao pos)
        {
            if (!Tab.ExistePeca(pos))
            {
                throw new Tabuleiro_Exception("Não existe peça nessa posição!");
            }
            if (Tab.peca(pos).cor != Jogador_Atual)
            {
                throw new Tabuleiro_Exception("Só pode mover as peças " + Jogador_Atual);
            }

            if (!Tab.peca(pos).exixteMovimentosPossiveis())
            {
                throw new Tabuleiro_Exception("Peça presa!");
            }
        }

        public void Valida_Pos_Destino(Posicao origem, Posicao destino)
        {
            if (!Tab.peca(origem).Movimento_Possivel(destino))
            {
                throw new Tabuleiro_Exception("Movimento inválido!");
            }
        }

        public HashSet<Peca> Pecas_Capturadas(Cor_Pecas cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca x in capturadas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> Pecas_Em_Jogo(Cor_Pecas cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas)
            {
                if (x.cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(Pecas_Capturadas(cor));
            return aux;
        }

        private Cor_Pecas adversaria(Cor_Pecas cor)
        {
            if (cor == Cor_Pecas.Branca)
            {
                return Cor_Pecas.Preta;
            }
            else return Cor_Pecas.Branca;
        }

        private Peca Rei(Cor_Pecas cor)
        {
            foreach (Peca x in Pecas_Em_Jogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }

        public bool Esta_Em_Xeque(Cor_Pecas cor)
        {
            Peca R = Rei(cor);
            if (R == null)
            {
                throw new Tabuleiro_Exception("Não existe Rei no tabuleiro");
            }

            foreach (Peca x in Pecas_Em_Jogo(adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool TesteXequemate(Cor_Pecas cor)
        {
            if (!Esta_Em_Xeque(cor))
            {
                return false;
            }
            foreach (Peca x in Pecas_Em_Jogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < Tab.linha; i++)
                {
                    for (int j = 0; j < Tab.coluna; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = Movimento(origem, destino);
                            bool testeXeque = Esta_Em_Xeque(cor);
                            Desfaz_Movimento(origem, destino, pecaCapturada);

                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }

            }
            return true;
        }

        public void Colocar_Nova_Peca(char coluna, int linha, Peca peca)
        {
            Tab.ColocarPeca(peca, new Posicao_Xadrez(coluna, linha).ToPosicao());
            pecas.Add(peca);
        }

        private void Colocar_Pecas()
        {
            //brancas
            Colocar_Nova_Peca('a', 1, new Torre(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('b', 1, new Cavalo(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('c', 1, new Bispo(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('d', 1, new Dama(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('e', 1, new Rei(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('f', 1, new Bispo(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('g', 1, new Cavalo(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('h', 1, new Torre(Cor_Pecas.Branca, Tab));
            Colocar_Nova_Peca('a', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('b', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('c', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('d', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('e', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('f', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('g', 2, new Peao(Cor_Pecas.Branca, Tab, this));
            Colocar_Nova_Peca('h', 2, new Peao(Cor_Pecas.Branca, Tab, this));

            //pretas
            Colocar_Nova_Peca('a', 8, new Torre(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('b', 8, new Cavalo(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('c', 8, new Bispo(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('d', 8, new Dama(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('e', 8, new Rei(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('f', 8, new Bispo(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('g', 8, new Cavalo(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('h', 8, new Torre(Cor_Pecas.Preta, Tab));
            Colocar_Nova_Peca('a', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('b', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('c', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('d', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('e', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('f', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('g', 7, new Peao(Cor_Pecas.Preta, Tab, this));
            Colocar_Nova_Peca('h', 7, new Peao(Cor_Pecas.Preta, Tab, this));



        }
    }
}
