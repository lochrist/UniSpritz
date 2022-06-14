using UniMini;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MQ
{
    public enum CardState
    {
        OutOfBoard,
        Hidden,
        SpiedByOpponent,
        SpiedByPlayer,
        SpiedByPlayers,
        Revealed,
        ReadiedForPlayer,
        ReadiedForOpponent,
    }

    class Card
    {
        public AnimSprite sprite;
    }

    class FusedCard
    {
        public FusedCard()
        {
            state = CardState.Hidden;
        }

        public Card opponentCard;
        public Card playerCard;
        public CardState state;
        public bool isActivated;

        public bool isVisible => state > CardState.SpiedByOpponent;
        public bool isOnBoard => state > CardState.OutOfBoard;
    }

    class Deck
    {
        public Card[] cards;
    }

    class Player
    {
        public string name;
        public int hp;
        public Deck deck;
    }

    static class Theme
    {
        public static Color gameBackgroundColor = new Color32(37,40,45, 255);
        public static Color cardBackgroundColor = new Color32(56,56,56,255);
        public static Color activatePlayerColor = new Color32(255, 255, 255, 128);

    }
}

public class MemoryQuest : SpritzGame
{
    public bool drawDebugRects;
    public bool debugMode;
    public bool debugShowAllCards;

    MQ.FusedCard[] m_Cards;
    int m_CurrentCardX;
    int m_CurrentCardY;
    int m_RevealedCardIndex1;
    int m_RevealedCardIndex2;
    int m_RevealedTimer;
    const int gridSize = 6;
    const int nbCards = gridSize * gridSize;
    const int deckSize = nbCards / 2;
    const int spriteSize = 48;
    const int cardWidth = 48;
    const int cardHeight = 96;

    MQ.Player m_Player;
    MQ.Player m_Opponent;
    MQ.Player m_CurrentPlayer;

    RectInt m_OpponentRect;
    RectInt m_BoardRect;
    RectInt m_PlayerRect;
    RectInt m_InspectorRect;

    SpriteFont m_Font;

    public override void InitializeSpritz()
    {
        Spritz.CreateLayer("Spritesheets/uf_heroes");
        m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
        m_CurrentCardX = m_CurrentCardY = 0;
        m_Cards = new MQ.FusedCard[nbCards];

        gameObject.GetComponent<Camera>().backgroundColor = Color.grey;

        InitPlayers();
        InitBoard();
        InitUI();
        InitBoard();
        InitFont();
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        if (Spritz.GetKeyDown(KeyCode.UpArrow) || Spritz.GetKeyDown(KeyCode.W))
        {
            MoveSelectionUp();
        }
        else if (Spritz.GetKeyDown(KeyCode.DownArrow) || Spritz.GetKeyDown(KeyCode.S))
        {
            MoveSelectionDown();
        }
        else if (Spritz.GetKeyDown(KeyCode.LeftArrow) || Spritz.GetKeyDown(KeyCode.A))
        {
            MoveSelectionLeft();
        }
        else if (Spritz.GetKeyDown(KeyCode.RightArrow) || Spritz.GetKeyDown(KeyCode.D))
        {
            MoveSelectionRight();
        }
        else if (m_RevealedTimer > 0)
        {
            m_RevealedTimer--;
            OnRevealedTimerDone();
        }
        else if (Spritz.GetKeyDown(KeyCode.KeypadEnter) || Spritz.GetKeyDown(KeyCode.Return) || Spritz.GetKeyDown(KeyCode.Space))
        {
            TryActivateCurrentCard();
        }
        else if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            debugShowAllCards = !debugShowAllCards;
        }

        for (var c = 0; c < m_Cards.Length; ++c)
        {
            if (m_Cards[c].isVisible)
            {
                GetCard(c).sprite.Update();
            }
        }
    }

    public override void DrawSpritz()
    {
        // TO CHECK: awkward clear to avoid the top layer overridding all the background

        Spritz.currentLayerId = 1;
        Spritz.Clear(Color.clear);

        Spritz.currentLayerId = 0;
        Spritz.Clear(MQ.Theme.gameBackgroundColor);

        if (drawDebugRects)
        {
            DrawDebugRect();
        }
        else
        {
            DrawPlayerInfo(m_OpponentRect, m_Opponent);
            DrawBoard();
            DrawInspector();
            DrawPlayerInfo(m_PlayerRect, m_Player);
        }
    }

    private void InitPlayers()
    {
        var allMonsters = ExampleUtils.GetUfHeroes(Spritz.GetSprites());
        ExampleUtils.Shuffle(allMonsters);

        var playerCards = new MQ.Card[deckSize];
        for (var i = 0; i < deckSize; ++i)
            playerCards[i] = new MQ.Card() { sprite = allMonsters[i] };

        var overlordCards = new MQ.Card[deckSize];
        for (var i = 0; i < deckSize; ++i)
            overlordCards[i] = new MQ.Card() { sprite = allMonsters[deckSize + i] };

        m_Player = new MQ.Player()
        {
            name = "Seb",
            deck = new MQ.Deck()
            {
                cards = playerCards
            },
            hp = 10
        };

        m_Opponent = new MQ.Player()
        {
            name = "Opponent",
            deck = new MQ.Deck()
            {
                cards = overlordCards
            },
            hp = 12
        };

        m_CurrentPlayer = m_Player;
    }

    private void InitBoard()
    {
        var cardIndex = 0;
        for (var i = 0; i < deckSize; i++, cardIndex += 2)
        {
            var c1 = new MQ.FusedCard()
            {
                opponentCard = m_Opponent.deck.cards[i],
                playerCard = m_Player.deck.cards[i],
            };

            var c2 = new MQ.FusedCard()
            {
                playerCard = m_Player.deck.cards[i],
                opponentCard = m_Opponent.deck.cards[i],
            };

            m_Cards[cardIndex] = c1;
            m_Cards[cardIndex + 1] = c2;
        }

        ExampleUtils.Shuffle(m_Cards);
    }

    private void InitUI()
    {
        var playerZoneHeight = 50;
        var padding = 5;
        var totalRect = new RectInt(0, 0, resolution.x, resolution.y);
        var boardZone = totalRect.CutLeft(gridSize * (spriteSize + 5));
        var overlordZone = totalRect.CutTop(playerZoneHeight);
        var playerZone = totalRect.CutBottom(playerZoneHeight);
        var inspectorZone = totalRect;
        m_OpponentRect = overlordZone.AddPadding(padding);
        m_PlayerRect = playerZone.AddPadding(padding);
        m_BoardRect = boardZone.AddPadding(padding);
        m_InspectorRect = inspectorZone.AddPadding(5);
    }

    private void InitFont()
    {
        Spritz.CreateLayer("Fonts/Weiholmir_GameMaker_sheet");
        var sprites = Spritz.GetSprites();
        m_Font = new SpriteFont(7, 7);
        m_Font.Add('!', (char)127, sprites, 1);
    }

    private void MoveSelectionUp()
    {
        m_CurrentCardY -= 1;
        if (m_CurrentCardY == -1)
            m_CurrentCardY = gridSize - 1;
    }

    private void MoveSelectionDown()
    {
        m_CurrentCardY = (m_CurrentCardY + 1) % gridSize;
    }


    private void MoveSelectionLeft()
    {
        m_CurrentCardX -= 1;
        if (m_CurrentCardX == -1)
            m_CurrentCardX = gridSize - 1;
    }

    private void MoveSelectionRight()
    {
        m_CurrentCardX = (m_CurrentCardX + 1) % gridSize;
    }

    private void TryActivateCurrentCard()
    {
        var currentCardIndex = GetCurrentCard();
        if (m_RevealedCardIndex1 != currentCardIndex &&
            !m_Cards[currentCardIndex].isActivated &&
            ((m_CurrentPlayer == m_Player && m_Cards[currentCardIndex].state != MQ.CardState.ReadiedForOpponent) ||
             (m_CurrentPlayer == m_Opponent && m_Cards[currentCardIndex].state != MQ.CardState.ReadiedForPlayer))
            )
        {
            if (m_Cards[currentCardIndex].state < MQ.CardState.Revealed)
                m_Cards[currentCardIndex].state = MQ.CardState.Revealed;

            m_Cards[currentCardIndex].isActivated = true;
            if (m_RevealedCardIndex1 == -1)
            {
                m_RevealedCardIndex1 = currentCardIndex;
            }
            else
            {
                m_RevealedCardIndex2 = currentCardIndex;
                if (m_RevealedCardIndex1 != currentCardIndex && GetCard(currentCardIndex).sprite.frames[0] == GetCard(m_RevealedCardIndex1).sprite.frames[0])
                {
                    // This is a match.
                    m_RevealedTimer = 20;
                    HandleMatch();
                }
                else
                {
                    m_RevealedTimer = 45;
                }
            }
        }
    }

    private void OnRevealedTimerDone()
    {
        if (m_RevealedTimer == 0)
        {
            m_Cards[m_RevealedCardIndex1].isActivated = false;
            m_Cards[m_RevealedCardIndex2].isActivated = false;

            if (GetCard(m_RevealedCardIndex1) == GetCard(m_RevealedCardIndex2))
            {
                // This is a match:
                m_Cards[m_RevealedCardIndex1].state = MQ.CardState.OutOfBoard;
                m_Cards[m_RevealedCardIndex2].state = MQ.CardState.OutOfBoard;
            }
            else
            {
                // No match
                if (m_Cards[m_RevealedCardIndex1].state < MQ.CardState.Revealed)
                    m_Cards[m_RevealedCardIndex1].state = MQ.CardState.Revealed;

                if (m_Cards[m_RevealedCardIndex1].state < MQ.CardState.Revealed)
                    m_Cards[m_RevealedCardIndex2].state = MQ.CardState.Revealed;

                // Check all Revealed cards if we have some that matches one of the 2 revealed cards. They become Ready:
                for (var i = 0; i < m_Cards.Length; ++i)
                {
                    if (m_Cards[i].state == MQ.CardState.Revealed)
                    {
                        var c = GetCard(i);
                        var r1 = GetCard(m_RevealedCardIndex1);
                        var r2 = GetCard(m_RevealedCardIndex2);
                        var newState = m_CurrentPlayer == m_Player ? MQ.CardState.ReadiedForPlayer : MQ.CardState.ReadiedForOpponent;
                        if (i != m_RevealedCardIndex1 && r1 == c)
                        {
                            m_Cards[m_RevealedCardIndex1].state = newState;
                            m_Cards[i].state = newState;
                        }
                        else if (i != m_RevealedCardIndex2 && r2 == c)
                        {
                            m_Cards[m_RevealedCardIndex2].state = newState;
                            m_Cards[i].state = newState;
                        }
                    }
                }

                GetCard(m_RevealedCardIndex1).sprite.Reset();
                GetCard(m_RevealedCardIndex2).sprite.Reset();
            }

            SwitchPlayer();
            m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
        }
    }

    private void HandleMatch()
    {
        if (m_CurrentPlayer == m_Opponent)
            m_Player.hp -= 2;
        else
            m_Opponent.hp -= 2;
    }

    private void DrawDebugRect()
    {
        Spritz.DrawRectangle(m_OpponentRect.x, m_OpponentRect.y, m_OpponentRect.width, m_OpponentRect.height, Spritz.palette[5], false);
        Spritz.DrawRectangle(m_PlayerRect.x, m_PlayerRect.y, m_PlayerRect.width, m_PlayerRect.height, Spritz.palette[2], false);
        Spritz.DrawRectangle(m_BoardRect.x, m_BoardRect.y, m_BoardRect.width, m_BoardRect.height, Spritz.palette[3], false);
        Spritz.DrawRectangle(m_InspectorRect.x, m_InspectorRect.y, m_InspectorRect.width, m_InspectorRect.height, Spritz.palette[4], false);
        
        // Spritz.DrawRectangle(0, 0, 600, 600, Spritz.palette[4], true);
    }

    private void DrawInspector()
    {
        if (m_CurrentCardY != -1 && m_CurrentCardX != -1)
        {
            var currentCardIndex = GetCurrentCard();
            var c = m_Cards[currentCardIndex];
            if (c.isVisible && c.isOnBoard)
            {
                c.opponentCard.sprite.Draw(m_InspectorRect.x, m_InspectorRect.y);
                c.playerCard.sprite.Draw(m_InspectorRect.x, m_InspectorRect.y + spriteSize + 5);

                if (debugMode)
                {
                    var activated = c.isActivated ? "- Activated" : "";
                    var status = $"{c.state} {activated}";
                    DrawText(status, m_InspectorRect.x, m_InspectorRect.y + 2 *spriteSize + 5);
                    Spritz.DrawRectangle(m_InspectorRect.x, m_InspectorRect.y, m_InspectorRect.width, m_InspectorRect.height, Spritz.palette[4], false);
                }
                
            }
        }
    }

    private void SwitchPlayer()
    {
        // Switch player:
        if (m_CurrentPlayer == m_Player)
            m_CurrentPlayer = m_Opponent;
        else
            m_CurrentPlayer = m_Player;
    }
    
    private void DrawPlayerInfo(RectInt rect, MQ.Player p)
    {
        var currentPlayer = m_CurrentPlayer == p ? "*" : "";
        DrawText($"{p.name}  {p.hp}  {currentPlayer}", rect.x, rect.y);
    }

    private void DrawText(string text, int x, int y)
    {
        var currentLayer = Spritz.currentLayerId;
        Spritz.currentLayerId = 1;
        Spritz.Print(m_Font, text, x, y, Color.white);
        Spritz.currentLayerId = currentLayer;
    }

    private void DrawBoard()
    {
        for (var cx = 0; cx < gridSize; ++cx)
        {
            for (var cy = 0; cy < gridSize; ++cy)
            {
                var x = cx * cardWidth + m_BoardRect.x;
                // var y = cy * (spriteSize *2);
                var y = cy * (cardHeight) + m_BoardRect.y;
                var i = cx + (cy * gridSize);
                if (m_Cards[i].isOnBoard)
                {
                    DrawCard(i, x, y);
                }

                if (m_CurrentCardX == cx && m_CurrentCardY == cy)
                {
                    Spritz.DrawRectangle(x, y, cardWidth, cardHeight, Color.yellow, false);
                }
                else if (m_Cards[i].isActivated)
                {
                    Spritz.DrawRectangle(x, y, cardWidth, cardHeight, Color.blue, false);
                }
            }
        }
    }

    private void DrawCard(int cardIndex, int x, int y)
    {
        Spritz.DrawRectangle(x + 1, y + 1, cardWidth - 2, cardHeight - 2, MQ.Theme.cardBackgroundColor, true);
        if (m_Cards[cardIndex].isVisible || debugShowAllCards)
        {
            var acttivatePlayerRect = new RectInt(x, y + (m_CurrentPlayer == m_Player ? 48 : 0), 48, 48).AddPadding(5);
            
            if (m_Cards[cardIndex].state == MQ.CardState.ReadiedForPlayer)
            {
                if (m_CurrentPlayer == m_Player)
                    Spritz.DrawRectangle(acttivatePlayerRect.x, acttivatePlayerRect.y, acttivatePlayerRect.width, acttivatePlayerRect.height, MQ.Theme.activatePlayerColor, true);
                m_Cards[cardIndex].playerCard.sprite.Draw(x, y + 48);
            }
            else if (m_Cards[cardIndex].state == MQ.CardState.ReadiedForOpponent)
            {
                if (m_CurrentPlayer == m_Opponent)
                    Spritz.DrawRectangle(acttivatePlayerRect.x, acttivatePlayerRect.y, acttivatePlayerRect.width, acttivatePlayerRect.height, MQ.Theme.activatePlayerColor, true);
                m_Cards[cardIndex].opponentCard.sprite.Draw(x, y);
            }
            else
            {
                Spritz.DrawRectangle(acttivatePlayerRect.x, acttivatePlayerRect.y, acttivatePlayerRect.width, acttivatePlayerRect.height, MQ.Theme.activatePlayerColor, true);
                m_Cards[cardIndex].opponentCard.sprite.Draw(x, y);
                m_Cards[cardIndex].playerCard.sprite.Draw(x, y + 48);
            }
        }
    }

    private MQ.Card GetCard(int i)
    {
        if (m_CurrentPlayer == m_Player)
            return m_Cards[i].playerCard;
        return m_Cards[i].opponentCard;
    }

    private int GetCurrentCard()
    {
        return m_CurrentCardX + m_CurrentCardY * gridSize;
    }
}
