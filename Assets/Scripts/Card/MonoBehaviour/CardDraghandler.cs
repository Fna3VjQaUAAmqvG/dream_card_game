using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDraghandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject arrowPrefab;
    private GameObject currentArrow;
    private Card currentCard;
    private bool canMove;
    private bool canExectute;
    private CharacterBase targetCharacter;
   
    private void Awake()
    {
        currentCard = GetComponent<Card>();
    }
    private void OnDisable() {
        canMove=false;
        canExectute=false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!currentCard.isAvailable)
            return;
        switch (currentCard.cardData.cardType)
        {
            case CardType.Attack:
                currentArrow = Instantiate(arrowPrefab, transform.position, quaternion.identity);
                break;
            case CardType.Defense:
            case CardType.Ability:
                canMove = true;
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!currentCard.isAvailable)
            return;
        if (canMove)
        {
            currentCard.isAnimating = true;
            Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            currentCard.transform.position = worldPos;
            canExectute = worldPos.y > 1f;
        }
        else
        {
            if (eventData.pointerEnter == null)
            {
                return;
            }
            if (eventData.pointerEnter.CompareTag("Enemy"))
            {
                canExectute = true;
                targetCharacter = eventData.pointerEnter.GetComponent<CharacterBase>();
                return;
            }
            canExectute = false;
            targetCharacter = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!currentCard.isAvailable)
            return;
        if (currentArrow != null)
            Destroy(currentArrow);

        if (canExectute)
        {
            currentCard.ExecuteCardEffects(currentCard.player,targetCharacter);
        }
        else
        {
            currentCard.ResetCardTransform();
            currentCard.isAnimating = false;
        }
    }
}
