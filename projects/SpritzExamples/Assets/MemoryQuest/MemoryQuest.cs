using UniMini;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    8x8 -> 
    16x8-> 16x48 => 768, 8*384
*/

namespace MQ
{
    class Card
    {
        public AnimSprite sprite;
    }

    class FusedCard
    {
        public Card c1;
        public Card c2;
        public bool isVisible;
        public bool isRevealed;
        public bool valid;
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
}

public class MemoryQuest : SpritzGame
{
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
    bool m_DebugMode;
    bool m_EasyMode;

    MQ.Player m_Player;
    MQ.Player m_Overlord;
    MQ.Player m_CurrentPlayer;

    RectInt m_OverlordRect;
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

        m_Overlord = new MQ.Player()
        {
            name = "Overlord",
            deck = new MQ.Deck()
            {
                cards = overlordCards
            },
            hp = 12
        };

        m_CurrentPlayer = m_Player;

        var cardIndex = 0;
        for (var i = 0; i < deckSize; i++, cardIndex += 2)
        {
            var c1 = new MQ.FusedCard()
            {
                c1 = m_Player.deck.cards[i],
                c2 = m_Overlord.deck.cards[i],
                isVisible = false,
                isRevealed = false,
                valid = true
            };

            var c2 = new MQ.FusedCard()
            {
                c1 = m_Player.deck.cards[i],
                c2 = m_Overlord.deck.cards[i],
                isVisible = false,
                isRevealed = false,
                valid = true
            };

            m_Cards[cardIndex] = c1;
            m_Cards[cardIndex + 1] = c2;
        }

        m_EasyMode = true;

        ExampleUtils.Shuffle(m_Cards);

        var playerZoneHeight = 100;
        var margin = 5;
        m_OverlordRect = new RectInt(margin, margin, resolution.x - 2 * margin, playerZoneHeight);
        m_PlayerRect = new RectInt(margin, resolution.y - (playerZoneHeight + margin*2), resolution.x - 2 * margin, playerZoneHeight);
        m_BoardRect = new RectInt(margin, m_OverlordRect.yMax, gridSize * (spriteSize + 5), resolution.y - m_OverlordRect.height - m_PlayerRect.height - 2 * margin);
        m_InspectorRect = new RectInt(m_BoardRect.xMax, m_OverlordRect.yMax, resolution.x - m_BoardRect.width, m_BoardRect.height);

        Spritz.CreateLayer("Fonts/Weiholmir_GameMaker_sheet");
        var sprites = Spritz.GetSprites();
        m_Font = new SpriteFont(7, 7);
        m_Font.Add('!', (char)127, sprites, 1);
    }

    public override void UpdateSpritz()
    {
        // Update objects behavior according to input
        if (Spritz.GetKeyDown(KeyCode.UpArrow) || Spritz.GetKeyDown(KeyCode.W))
        {
            m_CurrentCardY -= 1;
            if (m_CurrentCardY == -1)
                m_CurrentCardY = gridSize - 1;
        }
        else if (Spritz.GetKeyDown(KeyCode.DownArrow) || Spritz.GetKeyDown(KeyCode.S))
        {
            m_CurrentCardY = (m_CurrentCardY + 1) % gridSize;

        }
        else if (Spritz.GetKeyDown(KeyCode.LeftArrow) || Spritz.GetKeyDown(KeyCode.A))
        {
            m_CurrentCardX -= 1;
            if (m_CurrentCardX == -1)
                m_CurrentCardX = gridSize - 1;
        }
        else if (Spritz.GetKeyDown(KeyCode.RightArrow) || Spritz.GetKeyDown(KeyCode.D))
        {
            m_CurrentCardX = (m_CurrentCardX + 1) % gridSize;
        }
        else if (m_RevealedTimer > 0)
        {
            m_RevealedTimer--;
            if (m_RevealedTimer == 0)
            {
                if (GetCard(m_RevealedCardIndex1).sprite.frames[0] == GetCard(m_RevealedCardIndex2).sprite.frames[0])
                {
                    // This is a match:
                    m_Cards[m_RevealedCardIndex2].valid = false;
                    m_Cards[m_RevealedCardIndex1].valid = false;
                }
                else
                {
                    // No match
                    m_Cards[m_RevealedCardIndex1].isVisible = m_EasyMode;
                    m_Cards[m_RevealedCardIndex2].isVisible = m_EasyMode;
                    m_Cards[m_RevealedCardIndex1].isRevealed = false;
                    m_Cards[m_RevealedCardIndex2].isRevealed = false;
                    GetCard(m_RevealedCardIndex1).sprite.Reset();
                    GetCard(m_RevealedCardIndex2).sprite.Reset();
                }

                SwitchPlayer();
                m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
            }
        }
        else if (Spritz.GetKeyDown(KeyCode.KeypadEnter) || Spritz.GetKeyDown(KeyCode.Return) || Spritz.GetKeyDown(KeyCode.Space))
        {
            var currentCardIndex = m_CurrentCardX + m_CurrentCardY * gridSize;
            if (m_RevealedCardIndex1 != currentCardIndex && !m_Cards[currentCardIndex].isRevealed)
            {
                m_Cards[currentCardIndex].isRevealed = true;
                m_Cards[currentCardIndex].isVisible = true;
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
        else if (Spritz.GetKeyDown(KeyCode.Escape))
        {
            m_DebugMode = !m_DebugMode;
            for (var c = 0; c < m_Cards.Length; ++c)
            {
                if (m_Cards[c].valid)
                {
                    m_Cards[c].isVisible = m_DebugMode;
                    m_Cards[c].isRevealed = false;
                }
            }
            m_RevealedCardIndex1 = m_RevealedCardIndex2 = -1;
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
        Spritz.Clear(Color.grey);

        // DrawDebugRect();

        DrawPlayerInfo(m_OverlordRect, m_Overlord);
        DrawBoard();
        DrawInspector();
        DrawPlayerInfo(m_PlayerRect, m_Player);
    }

    private void HandleMatch()
    {
        if (m_CurrentPlayer == m_Overlord)
            m_Player.hp -= 2;
        else
            m_Overlord.hp -= 2;
    }

    private void DrawDebugRect()
    {
        Spritz.DrawRectangle(m_OverlordRect.x, m_OverlordRect.y, m_OverlordRect.width, m_OverlordRect.height, Spritz.palette[1],false);
        Spritz.DrawRectangle(m_PlayerRect.x, m_PlayerRect.y, m_PlayerRect.width, m_PlayerRect.height, Spritz.palette[2], false);
        Spritz.DrawRectangle(m_BoardRect.x, m_BoardRect.y, m_BoardRect.width, m_BoardRect.height, Spritz.palette[3], false);
        Spritz.DrawRectangle(m_InspectorRect.x, m_InspectorRect.y, m_InspectorRect.width, m_InspectorRect.height, Spritz.palette[4], false);
    }

    private void DrawInspector()
    {
        if (m_CurrentCardY != -1 && m_CurrentCardX != -1)
        {
            var currentCardIndex = m_CurrentCardX + m_CurrentCardY * gridSize;
            var c = m_Cards[currentCardIndex];
            if (c.isVisible)
            {
                c.c1.sprite.Draw(m_InspectorRect.x, m_InspectorRect.y);
                c.c2.sprite.Draw(m_InspectorRect.x, m_InspectorRect.y + spriteSize + 5);
                Spritz.DrawRectangle(m_InspectorRect.x, m_InspectorRect.y, m_InspectorRect.width, m_InspectorRect.height, Spritz.palette[4], false);
            }
        }
    }

    private void SwitchPlayer()
    {
        // Switch player:
        if (m_CurrentPlayer == m_Player)
            m_CurrentPlayer = m_Overlord;
        else
            m_CurrentPlayer = m_Player;
    }
    
    private void DrawPlayerInfo(RectInt rect, MQ.Player p)
    {
        var currentPlayer = m_CurrentPlayer == p ? "*" : "";
        DrawText($"{p.name}  {p.hp}  {currentPlayer}", rect.x, rect.y, Color.red);
    }

    private void DrawText(string text, int x, int y, Color c)
    {
        var currentLayer = Spritz.currentLayerId;
        Spritz.currentLayerId = 1;
        Spritz.Print(m_Font, text, x, y, c);
        Spritz.currentLayerId = currentLayer;
    }

    private void DrawBoard()
    {
        for (var cx = 0; cx < gridSize; ++cx)
        {
            for (var cy = 0; cy < gridSize; ++cy)
            {
                var x = cx * spriteSize + m_BoardRect.x;
                // var y = cy * (spriteSize *2);
                var y = cy * (spriteSize) + m_BoardRect.y;
                var i = cx + (cy * gridSize);
                if (m_Cards[i].valid)
                {
                    if (m_Cards[i].isVisible)
                    {
                        GetCard(i).sprite.Draw(x, y);
                    }
                    else
                    {
                        Spritz.DrawRectangle(x + 1, y + 1, spriteSize - 2, spriteSize - 2, Color.blue, false);
                    }
                }
                if (m_CurrentCardX == cx && m_CurrentCardY == cy)
                {
                    // Spritz.DrawRectangle(x, y, spriteSize, 2 * spriteSize, Color.yellow, false);
                    Spritz.DrawRectangle(x, y, spriteSize, spriteSize, Color.yellow, false);
                }
            }
        }
    }

    private MQ.Card GetCard(int i)
    {
        if (m_CurrentPlayer == m_Player)
            return m_Cards[i].c1;
        return m_Cards[i].c2;
    }
}
