import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'countryFlag', standalone: true })
export class CountryFlagPipe implements PipeTransform {
  private readonly keywords: Record<string, string> = {
    'australian': 'AU',
    'australian grand prix': 'AU',
    'bahrain': 'BH',
    'saudi arabian': 'SA',
    'japanese': 'JP',
    'chinese': 'CN',
    'miami': 'US',
    'emilia': 'IT',
    'espana': 'ES',
    'monaco': 'MC',
    'canadian': 'CA',
    'canada': 'CA',
    'spanish': 'ES',
    'austrian': 'AT',
    'british': 'GB',
    'hungarian': 'HU',
    'belgian': 'BE',
    'dutch': 'NL',
    'italian': 'IT',
    'italia': 'IT',
    'azerbaijani': 'AZ',
    'azerbaijan': 'AZ',
    'singapore': 'SG',
    'united states': 'US',
    'mexican': 'MX',
    'mexico': 'MX',
    'brazilian': 'BR',
    'paulo': 'BR',
    'las vegas': 'US',
    'qatar': 'QA',
    'abu dhabi': 'AE',
  };

  transform(gpName: string): string {
    const lower = gpName.toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');

    for (const [keyword, code] of Object.entries(this.keywords)) {
      const normalizedKeyword = keyword
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '');

      if (lower.includes(normalizedKeyword)) {
        return this.codeToEmoji(code);
      }
    }
    return '🏁';
  }

  private codeToEmoji(code: string): string {
    return [...code.toUpperCase()]
      .map(c => String.fromCodePoint(0x1F1E6 - 65 + c.charCodeAt(0)))
      .join('');
  }
}
