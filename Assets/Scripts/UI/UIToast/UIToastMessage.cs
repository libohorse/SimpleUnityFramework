using System.Collections;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIToast
{
    public class UIToastMessage : UINode
    {
        public Text messageText;

        public void Initialize(string message)
        {
            messageText.text = message;
            StartCoroutine(SimpleToastAnimation());
        }

        private IEnumerator SimpleToastAnimation()
        {
            const float showDuration = 1f;
            const float moveDuration = 1f;
            const float moveDistance = 400f;
            var elapsed = 0f;
            var startPos = transform.localPosition;
            var endPos = startPos + new Vector3(0, moveDistance, 0);

            // 显示1秒
            yield return new WaitForSeconds(showDuration);

            // 1秒内往上移动400px并逐渐消失
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / moveDuration);
                transform.localPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}