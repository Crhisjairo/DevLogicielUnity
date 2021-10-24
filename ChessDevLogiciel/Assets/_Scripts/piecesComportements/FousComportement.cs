using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FousComportement : Piece
{

    private Vector2Int[] _moveSet = new Vector2Int[]
    {
        new Vector2Int(1, 1), //move pour manger en diagonal droit vers le haut
        new Vector2Int(-1, 1), //move pour manger en diagonal gauche vers le haut
        new Vector2Int(1, -1), //move pour manger en diagonal droite vers le bas
        new Vector2Int(-1, -1) //move pour manger en diagonal gauche vers le bas
    };

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
        _outline = GetComponent<Outline>();
        _outline.enabled = false; //On cache le outline au début.

        //On d�finit l'ensemble de mouvement de la pi�ce
        moveSet = _moveSet;
    }

    public override void SelectionnerPiece()
    {
        //On peut changer la couleur de la pi�ce icitte si l'on veut
        EstSelectionne = true; //On marque la pi�ce comme s�l�ctionn�e
        caseActuelle.SetEstActive(true); //On active la case o� se trouve cette pi�ce pour l'allumer
        
        /*
         * Il faut permettre seulement les d�placements possibles ici selon le type de pi�ce.
         * C'est le BoardManager qui activera les cases (pour que la pi�ce puisse se d�placer) selon le moveSet envoy�.
         *
         * Le moveSet pourra �tre modifi� avant d'activer les cases. Par exemple, le pion change son moveSet (ensemble de mouvements)
         * si une pi�ce se trouve dans un coin (car le pion mange en diagonale).
         * 
         * Dans ce cas, c'est un Pion, alors la logique du pion sera unique. Pourtant, vous pouvez vous baser sur cette
         * logique pour l'impl�menter dans les autres pi�ces.
         */

        Vector2Int coordonneesDeCetteCase = new Vector2Int();
        Joueur.NumeroJoueur joueurActuel = PlayersController.Instance._joueurActive.numeroJoueur;
        //On va utiliser les coodonn�es d'une case relative au joueur. Si on ajoute plus de joueurs, le code reste flexible
        if (joueurActuel is Joueur.NumeroJoueur.Joueur1)
        {
            coordonneesDeCetteCase = caseActuelle.coordonneesDeCasePourBlanc;
        }
        else if (joueurActuel is Joueur.NumeroJoueur.Joueur2)
        {
            coordonneesDeCetteCase = caseActuelle.coordonneesDeCasePourNoir;
        }

        //C'EST TOUTE CETTE PARTIE qui change SELON la pi�ce

        for (int moveSetIndex = 0; moveSetIndex < moveSet.Length; moveSetIndex++)
        {
            Vector2Int nextMove = moveSet[moveSetIndex];
            nextMove += coordonneesDeCetteCase; //Pour conna�tre le mouvement r�latif � la position de cette case
        
            Piece pieceInNextCase; //Pièce qui va être retrouvé si jamais HasPieceOnCoord retourn vrai.

            //Pour la diagonal ver le haut droit
            for (int yPosi = coordonneesDeCetteCase.y; yPosi <= BoardManager.MAX_BOARD_SIZE; yPosi++)
            {
                //Debug.Log(nextMove);
                
                if (BoardManager.Instance.HasPieceOnCoord(nextMove.x, nextMove.y, out pieceInNextCase))
                {
                    //S'il s'agit d'un ennemie
                    if (pieceInNextCase.JoueurProprietaire != joueurActuel)
                    {
                        //On active les cases par coordonn�es dans le board
                        BoardManager.Instance.ActiverCaseByCoord(nextMove.x, nextMove.y, true, joueurActuel);
                        break;
                    }
                    else //S'il s'agit de nous même, on s'arrête
                    {
                        break;
                    }
                }
                else
                {
                    //On active les cases par coordonn�es dans le board
                    BoardManager.Instance.ActiverCaseByCoord(nextMove.x, nextMove.y, true, joueurActuel);
                }
            
                nextMove += moveSet[moveSetIndex];
            }
        }
        
    }

    public override void DeplacerPiece(Case caseDestination)
    {
        //Dans le cas qu'il ait une pi�ce dans la case qu'on veut se d�placer,
        //on check si l'on peut la manger ou si c'est une de nos piece
        //pour ensuite se d�placer 

        //On d�place
        //On remplace la coordonn�e  y  pour qu'elle reste intacte
        Vector3 destination = caseDestination.transform.position;

        destination.y = transform.position.y;
        _rb.MovePosition(destination);

        //Finalemment,
        //On efface la r�ference de la pi�ce dans la case
        //et on ajoute la ref�rence de cette pi�ce � la case o� l'on se d�place
        //caseDestination.SetPieceDansLaCase(this);
    }

    public override void DeselectionnerPiece()
    {
        EstSelectionne = false;

        //On dit au board de d�sactiver les cases actives
        BoardManager.Instance.DesactiverCases(); //le this est temporel
    }
}