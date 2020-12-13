using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordTextLocalizer : TextLocalizer
{
    public override void UpdateLocalization()
    {
        textMeshProText.SetText(KeywordsReplacer.ReplaceKeywords(Localization.GetLocalizedString(text.key)));
    }
}
