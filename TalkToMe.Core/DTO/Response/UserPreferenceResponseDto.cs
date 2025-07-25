﻿namespace TalkToMe.Core.DTO.Response
{
    public class UserPreferenceResponseDto
    {
        public string UserId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string NativeLanguage { get; set; } = default!;
        public string PreferedPronoun { get; set; } = default!;
        public string CurrentLanguageToLearn { get; set; } = default!;
    }
}
