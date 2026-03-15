import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'countryFlag', standalone: true })
export class CountryFlagPipe implements PipeTransform {
  private readonly keywords: Record<string, string> = {
    'australian': 'AU',
    'australia': 'AU',
    'america': 'US',
    'mellbourne': 'AU',
    "barcelona": 'ESP',
    "madrid": 'ESP',
    'australian grand prix': 'AU',
    'bahrain': 'BH',
    'szahir': 'BH',
    'saudi arabian': 'SA',
    'saudi arabia': 'SA',
    'jeddah': 'SA',
    'dzsidda': 'SA',
    'japanese': 'JP',
    'japan': 'JP',
    'chinese': 'CN',
    'china': 'CN',
    'miami': 'US',
    'emilia': 'IT',
    'espana': 'ES',
    'monaco': 'MC',
    'monte-carlo': 'MC',
    'canadian': 'CA',
    'canada': 'CA',
    'spanish': 'ES',
    'spielberg': 'AT',
    'spain': 'ES',
    'austrian': 'AT',
    'austria': 'AT',
    'british': 'GB',
    'great britain': 'GB',
    'silverstone': 'GB',
    'hungarian': 'HU',
    'hungary': 'HU',
    'hungaroring': 'HU',
    'mogyorod': 'HU',
    'belgian': 'BE',
    'belgium': 'BE',
    'stavelot': 'BE',
    'dutch': 'NL',
    'netherlands': 'NL',
    'italian': 'IT',
    'italia': 'IT',
    'italy': 'IT',
    'monza': 'IT',
    'azerbaijani': 'AZ',
    'azerbaijan': 'AZ',
    'baku': 'AZ',
    'singapore': 'SG',
    'united states': 'US',
    'usa': 'US',
    'mexican': 'MX',
    'mexico': 'MX',
    'brazilian': 'BR',
    'brazilia': 'BR',
    'paulo': 'BR',
    'las vegas': 'US',
    'qatar': 'QA',
    'loszail': 'QA',
    'abu dhabi': 'AE',
    'abu-dzabi': 'AE',
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
