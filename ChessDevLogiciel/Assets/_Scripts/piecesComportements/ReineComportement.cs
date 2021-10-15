using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

public class ReineComportement : Piece
{
    private Rigidbody _rb;

    private bool isFirstmove;

    [SerializeField] private Vector2Int[] _moveSet = new Vector2Int[]
    {    
        new Vector2Int(0, 1), //Move vers en avant de 1 case [0]
        new Vector2Int(0, 2), //Move vers en avant de 2 case [1]
        new Vector2Int(1, 1),  //move pour manger en diagonal droit [2]
        new Vector2Int(-1, 1),  //move pour manger en diagonal gauche [3]
        
        
        
        
        new Vector2Int(BoardManager.MAX_BOARD_SIZE, 0), //Mouvement vers toute la droite [4]
        new Vector2Int(-BoardManager.MAX_BOARD_SIZE, 0), //Mouvement vers toute la gauche [5]
        new Vector2Int(0,BoardManager.MAX_BOARD_SIZE),//Mouvement vers tous en haut [6]
        new Vector2Int(0,-BoardManager.MAX_BOARD_SIZE),//Mouvement vers tous en bas [7]
        
        
        
    
        
        
        new Vector2Int(1, 1), //move pour manger en diagonal droit vers le haut [8]
        new Vector2Int(-1, 1), //move pour manger en diagonal gauche vers le haut [9]
        new Vector2Int(1, -1), //move pour manger en diagonal droite vers le bas [10]
        new Vector2Int(-1, -1) //move pour manger en diagonal gauche vers le bas [11]
        
        
        
        
        
    };
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        moveSet = _moveSet;
        isFirstmove = true;
    }
    public override void SelectionnerPiece()
    {
        //On peut changer la couleur de la pièce si l'on veut
        caseActuelle.SetEstActive(true);
        EstSelectionne = true;
        
        //Il faut permettre seulement les déplacements possibles ici selon le type de pièce.
        //C'est le BoardManager qui activera les cases pour se déplacer selon le moveSet envoyé.

        Vector2Int coordonneesDeCetteCase = new Vector2Int();
        Joueur.NumeroJoueur numeroJoueur = PlayersController.Instance._joueurActive.numeroJoueur;


        if (numeroJoueur is Joueur.NumeroJoueur.Joueur1)
        {
            coordonneesDeCetteCase = caseActuelle.coordonneesDeCasePourBlanc;
        }
        else if(numeroJoueur is Joueur.NumeroJoueur.Joueur2)
        {
            coordonneesDeCetteCase = caseActuelle.coordonneesDeCasePourNoir;
        }

        //Mouvement du pion
        if (isFirstmove)
        {
            Vector2Int firstMove = moveSet[1];
            firstMove = coordonneesDeCetteCase;

            for (int y = coordonneesDeCetteCase.y; y <= firstMove.y; y++)
            {
                BoardManager.Instance.ActiverCaseByCoord(firstMove.x,y,true,numeroJoueur);
            }
            
        }
        Vector2Int nextDiagonalMoveRight = moveSet[2]; //On utilise le move pour manger
        Vector2Int nextDiagonalMoveLeft = moveSet[3]; //On utilise le move pour manger
        nextDiagonalMoveRight += coordonneesDeCetteCase; //Pour connaître le mouvement rélatif à la position de cette case
        nextDiagonalMoveLeft += coordonneesDeCetteCase; //Pour connaître le mouvement rélatif à la position de cette case

        if (BoardManager.Instance.HasPieceOnCoord(nextDiagonalMoveRight.x, nextDiagonalMoveRight.y))
        {
            BoardManager.Instance.ActiverCaseByCoord(nextDiagonalMoveRight.x, nextDiagonalMoveRight.y, true, numeroJoueur);
        }
      
        if (BoardManager.Instance.HasPieceOnCoord(nextDiagonalMoveLeft.x, nextDiagonalMoveLeft.y))
        {
            BoardManager.Instance.ActiverCaseByCoord(nextDiagonalMoveLeft.x, nextDiagonalMoveLeft.y, true, numeroJoueur);
        }
        
        // Mouvement de la tour

        Vector2Int nextMove;
        for (int i = 4; i < 7; i++)
        {
            nextMove = moveSet[i];
            nextMove += coordonneesDeCetteCase; //Pour connaître le mouvement rélatif à la position de cette case


            //On active dans les coordonnées x positif
            for (int xPosi = coordonneesDeCetteCase.x; xPosi <= nextMove.x; xPosi++)
            {
                if (BoardManager.Instance.HasPieceOnCoord(xPosi, nextMove.y) && xPosi != coordonneesDeCetteCase.x)
                {
                    break;
                }

                //On active les cases par coordonnées dans le board
                BoardManager.Instance.ActiverCaseByCoord(xPosi, coordonneesDeCetteCase.y, true, numeroJoueur);
            }

            //On active dans les coordonnées x negatif
            for (int xNega = coordonneesDeCetteCase.x; xNega >= nextMove.x; xNega--)
            {
                if (BoardManager.Instance.HasPieceOnCoord(xNega, nextMove.y) && xNega != coordonneesDeCetteCase.x)
                {
                    break;
                }

                //On active les cases par coordonnées dans le board
                BoardManager.Instance.ActiverCaseByCoord(xNega, coordonneesDeCetteCase.y, true, numeroJoueur);
            }

            //On active dans les coordonnées y positif
            for (int yPosi = coordonneesDeCetteCase.y; yPosi <= nextMove.y; yPosi++)
            {
                if (BoardManager.Instance.HasPieceOnCoord(coordonneesDeCetteCase.x, yPosi) &&
                    yPosi != coordonneesDeCetteCase.y)
                {
                    break;
                }

                //On active les cases par coordonnées dans le board
                BoardManager.Instance.ActiverCaseByCoord(nextMove.x, yPosi, true, numeroJoueur);
                Debug.Log(nextMove.x + ":" + yPosi);
            }

            //On active dans les coordonnées y negatif
            for (int yPosi = coordonneesDeCetteCase.y; yPosi >= nextMove.y; yPosi--)
            {
                if (BoardManager.Instance.HasPieceOnCoord(coordonneesDeCetteCase.x, yPosi) &&
                    yPosi != coordonneesDeCetteCase.y)
                {
                    break;
                }

                //On active les cases par coordonnées dans le board
                BoardManager.Instance.ActiverCaseByCoord(nextMove.x, yPosi, true, numeroJoueur);
                Debug.Log(nextMove.x + ":" + yPosi);
            }

        }
        
        
        // Mouvement fou
        nextMove = moveSet[8];
        nextMove += coordonneesDeCetteCase; //Pour conna�tre le mouvement r�latif � la position de cette case

        //Pour la diagonal ver le haut droit
        for (int yPosi = coordonneesDeCetteCase.y; yPosi <= BoardManager.MAX_BOARD_SIZE; yPosi++)
        {
            //On active les cases par coordonn�es dans le board
            BoardManager.Instance.ActiverCaseByCoord(nextMove.x, nextMove.y, true, numeroJoueur);
            Debug.Log(nextMove.x + ":" + nextMove.y);

            nextMove += moveSet[8];
        }

        nextMove = moveSet[9];
        nextMove += coordonneesDeCetteCase;
        //Pour la diagonal ver le haut gauche
        for (int yPosi = coordonneesDeCetteCase.y; yPosi <= BoardManager.MAX_BOARD_SIZE; yPosi++)
        {
            //On active les cases par coordonn�es dans le board
            BoardManager.Instance.ActiverCaseByCoord(nextMove.x, nextMove.y, true, numeroJoueur);
            Debug.Log(nextMove.x + ":" + nextMove.y);

            nextMove += moveSet[9];
        }
        
        nextMove = moveSet[10];
        nextMove += coordonneesDeCetteCase;
        //Pour la diagonal ver le bas droite
        for (int yNegi = coordonneesDeCetteCase.y; yNegi >= -BoardManager.MAX_BOARD_SIZE; yNegi--)
        {
            //On active les cases par coordonn�es dans le board
            BoardManager.Instance.ActiverCaseByCoord(nextMove.x, nextMove.y, true, numeroJoueur);
            Debug.Log(nextMove.x + ":" + nextMove.y);

            nextMove += moveSet[10];
        }

        nextMove = moveSet[9];
        nextMove += coordonneesDeCetteCase;
        //Pour la diagonal ver le bas droite
        for (int yNegi = coordonneesDeCetteCase.y; yNegi >= -BoardManager.MAX_BOARD_SIZE; yNegi--)
        {
            //On active les cases par coordonn�es dans le board
            BoardManager.Instance.ActiverCaseByCoord(nextMove.x, nextMove.y, true, numeroJoueur);
            Debug.Log(nextMove.x + ":" + nextMove.y);

            nextMove += moveSet[11];
        }

    }
    



// Update is called once per frame
    public override void DeplacerPiece(Case caseDestination)
    {
        Vector3 destination = caseDestination.transform.position;

        destination.y = transform.position.y;
        _rb.MovePosition(destination);

    }

    

    public override void DeselectionnerPiece()
    {
        EstSelectionne = false;
        
        BoardManager.Instance.DesactiverCases();

    }

    
}
