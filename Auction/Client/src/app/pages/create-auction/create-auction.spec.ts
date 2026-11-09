import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { CreateAuction } from './create-auction';

describe('CreateAuction', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateAuction],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  it('renders the auction fields and photo picker', async () => {
    const fixture = TestBed.createComponent(CreateAuction);
    await fixture.whenStable();
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('h1')?.textContent).toContain('Создать аукцион');
    expect(element.querySelector('#productName')).not.toBeNull();
    expect(element.querySelector('#startsAt')).not.toBeNull();
    expect(element.querySelector('#endsAt')).not.toBeNull();
    expect(element.querySelector('input[type="file"]')).not.toBeNull();
  });

  it('shows required-field errors when submitted empty', async () => {
    const fixture = TestBed.createComponent(CreateAuction);
    await fixture.whenStable();
    fixture.detectChanges();

    fixture.nativeElement.querySelector('form')?.dispatchEvent(
      new Event('submit', { bubbles: true, cancelable: true }),
    );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Укажите название товара.');
    expect(fixture.nativeElement.textContent).toContain('Укажите дату и время начала.');
    expect(fixture.nativeElement.textContent).toContain('Укажите дату и время окончания.');
  });

  it('rejects photo extensions outside the allowed image formats', async () => {
    const fixture = TestBed.createComponent(CreateAuction);
    await fixture.whenStable();
    fixture.detectChanges();

    const photoInput = fixture.nativeElement.querySelector(
      'input[type="file"]',
    ) as HTMLInputElement;
    const file = new File(['not an image'], 'document.txt');
    Object.defineProperty(photoInput, 'files', {
      configurable: true,
      value: { item: (index: number) => (index === 0 ? file : null) },
    });
    photoInput.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain(
      'Выберите изображение в формате JPG, PNG или WebP.',
    );
  });
});
