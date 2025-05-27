using Microsoft.JSInterop;

namespace FrontEnd.Pages;

public partial class Document
{
    private int m_iCurrentSlide = 1;
    private int m_iTotalSlides = 9;

    private bool m_bCanPrevious => m_iCurrentSlide > 1;
    private bool m_bCanNext => m_iCurrentSlide < m_iTotalSlides;

    protected override async Task OnAfterRenderAsync(bool bFirstRender)
    {
        if (bFirstRender)
        {
            await JS.InvokeVoidAsync("eval", @"
                document.addEventListener('keydown', function(event) {
                    switch(event.key) {
                        case 'ArrowRight':
                        case ' ':
                            event.preventDefault();
                            DotNet.invokeMethodAsync('FrontEnd', 'NextSlideFromJS');
                            break;
                        case 'ArrowLeft':
                            event.preventDefault();
                            DotNet.invokeMethodAsync('FrontEnd', 'PreviousSlideFromJS');
                            break;
                    }
                });
            ");
        }
    }

    private void NextSlide()
    {
        if (m_bCanNext)
        {
            var currentSlideElement = $".slide:nth-child({m_iCurrentSlide})";
            var nextSlideElement = $".slide:nth-child({m_iCurrentSlide + 1})";

            m_iCurrentSlide++;
            UpdateSlideDisplay();
        }
    }

    private void PreviousSlide()
    {
        if (m_bCanPrevious)
        {
            m_iCurrentSlide--;
            UpdateSlideDisplay();
        }
    }

    private void GoToSlide(int slideNumber)
    {
        if (slideNumber >= 1 && slideNumber <= m_iTotalSlides)
        {
            m_iCurrentSlide = slideNumber;
            UpdateSlideDisplay();
        }
    }

    private void UpdateSlideDisplay()
    {
        StateHasChanged();

        JS.InvokeVoidAsync("eval", $@"
            document.querySelectorAll('.slide').forEach((slide, index) => {{
                slide.classList.remove('active');
                if (index === {m_iCurrentSlide - 1}) {{
                    slide.classList.add('active');
                }}
            }});
            document.querySelectorAll('.slide-number').forEach(num => {{
                num.textContent = '{m_iCurrentSlide} / {m_iTotalSlides}';
            }});
        ");
    }
}