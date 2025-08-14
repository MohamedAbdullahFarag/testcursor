# Popup Component System - Usage Guide

## Overview

The Ikhtibar project now has a consolidated popup system that replaces the previous multiple modal components with a single, highly configurable `Popup` component. This system provides:

- **Unified API** across all popup types
- **Comprehensive RTL/LTR support** for internationalization
- **Enhanced animations** and transitions
- **Better accessibility** features
- **Consistent styling** through CSS custom properties
- **Performance optimizations** with proper cleanup

## Components

### 1. Popup (Primary Component)

The main popup component with full feature set:

```tsx
import { Popup } from '@/shared/components';

const MyComponent = () => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <Popup
      isOpen={isOpen}
      onClose={() => setIsOpen(false)}
      title="Create New User"
      description="Fill out the form to create a new user account"
      size="lg"
      animation="scale"
      closeOnEscape={true}
      closeOnOverlayClick={true}
    >
      <UserForm onSubmit={handleSubmit} />
    </Popup>
  );
};
```

### 2. RTLModal (Legacy Wrapper)

Maintains backward compatibility with existing code:

```tsx
import { RTLModal } from '@/shared/components';

// Existing code continues to work
<RTLModal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  title="Edit User"
  size="4xl"
>
  <UserEditForm />
</RTLModal>
```

### 3. usePopup Hook

Simplifies popup state management:

```tsx
import { usePopup } from '@/shared/hooks/usePopup';

const MyComponent = () => {
  const popup = usePopup({
    onOpen: () => console.log('Popup opened'),
    onClose: () => console.log('Popup closed'),
    autoClose: 5000 // Auto-close after 5 seconds
  });

  return (
    <>
      <button onClick={popup.open}>Open Popup</button>
      
      <Popup
        isOpen={popup.isOpen}
        onClose={popup.close}
        title="Auto-closing Popup"
      >
        This popup will auto-close in 5 seconds
      </Popup>
    </>
  );
};
```

## API Reference

### Popup Props

| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `isOpen` | `boolean` | - | Whether the popup is open |
| `onClose` | `() => void` | - | Callback fired when popup should close |
| `title` | `string` | - | Popup title |
| `description` | `string` | - | Popup description (for accessibility) |
| `children` | `ReactNode` | - | Popup content |
| `size` | `'sm' \| 'md' \| 'lg' \| 'xl' \| '2xl' \| '4xl'` | `'md'` | Size variant |
| `closeOnOverlayClick` | `boolean` | `true` | Whether clicking overlay closes popup |
| `closeOnEscape` | `boolean` | `true` | Whether escape key closes popup |
| `showCloseButton` | `boolean` | `true` | Whether to show close button |
| `className` | `string` | `''` | Additional CSS classes for content |
| `overlayClassName` | `string` | `''` | Additional CSS classes for overlay |
| `animation` | `'fade' \| 'scale' \| 'none'` | `'scale'` | Animation variant |
| `portalContainer` | `HTMLElement` | `document.body` | Custom portal container |
| `onOpen` | `() => void` | - | Callback fired when popup opens |
| `onClosed` | `() => void` | - | Callback fired after popup closes |
| `zIndex` | `number` | - | Z-index override |

### usePopup Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `defaultOpen` | `boolean` | `false` | Initial open state |
| `onOpen` | `() => void` | - | Callback fired when popup opens |
| `onClose` | `() => void` | - | Callback fired when popup closes |
| `autoClose` | `number` | - | Auto-close after specified milliseconds |

## Size Variants

### Visual Reference

- **sm**: 384px (24rem) - Simple confirmations
- **md**: 448px (28rem) - Standard forms
- **lg**: 512px (32rem) - Detailed forms  
- **xl**: 576px (36rem) - Complex forms
- **2xl**: 672px (42rem) - Multi-step workflows
- **4xl**: 896px (56rem) - Full-featured interfaces

## Animation Options

### scale (Default)
- Smooth scale-in/out effect
- Best for most use cases
- Provides good visual feedback

### fade
- Simple opacity transition
- Subtle and unobtrusive
- Good for tooltips and notifications

### none
- No animation
- Instant show/hide
- Use for performance-critical scenarios

## Styling Customization

### CSS Custom Properties

Override the default styles using CSS custom properties:

```css
:root {
  --popup-z-modal: 10000;
  --popup-backdrop-blur: 8px;
  --popup-max-height: 95vh;
  --popup-transition-duration: 300ms;
  --popup-overlay-bg: rgba(0, 0, 0, 0.6);
  --popup-border-radius: 1rem;
}
```

### Custom Classes

Add custom styling through className props:

```tsx
<Popup
  className="my-custom-popup"
  overlayClassName="my-custom-overlay"
>
  Content
</Popup>
```

```css
.my-custom-popup {
  border: 2px solid #3b82f6;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.my-custom-overlay {
  background: rgba(0, 0, 0, 0.8);
  backdrop-filter: blur(10px);
}
```

## Migration Guide

### From Modal.tsx

```tsx
// Before
<Modal
  isOpen={isOpen}
  onClose={onClose}
  title="My Modal"
  size="lg"
>
  Content
</Modal>

// After
<Popup
  isOpen={isOpen}
  onClose={onClose}
  title="My Modal"
  size="lg"
>
  Content
</Popup>
```

### From RTLModal.tsx

RTLModal is now a wrapper around Popup, so existing code continues to work without changes. However, for new code, consider using Popup directly:

```tsx
// Existing code (still works)
<RTLModal
  isOpen={isOpen}
  onClose={onClose}
  title="My Modal"
>
  Content
</RTLModal>

// New recommended approach
<Popup
  isOpen={isOpen}
  onClose={onClose}
  title="My Modal"
>
  Content
</Popup>
```

### From RTLDialog.tsx

RTLDialog wraps mada-design-system components and remains unchanged for compatibility with existing library usage.

## Performance Considerations

### Portal Rendering
- All popups render in portals for proper z-index stacking
- Minimal DOM manipulation for better performance
- Automatic cleanup on unmount

### Animation Optimization
- Uses CSS transforms for hardware acceleration
- Supports `prefers-reduced-motion` for accessibility
- Configurable animation duration and easing

### Memory Management
- Automatic cleanup of event listeners
- Proper focus restoration
- Body scroll restoration on unmount

## Accessibility Features

### Keyboard Navigation
- **Escape key**: Closes popup (configurable)
- **Tab trapping**: Focus stays within popup
- **Focus restoration**: Returns focus to trigger element

### Screen Reader Support
- **ARIA attributes**: Proper dialog semantics
- **Announcements**: State changes announced
- **Descriptions**: Optional description for context

### High Contrast Mode
- **Border enhancement**: Better visibility
- **Background adjustment**: Improved contrast

## RTL/LTR Support

### Automatic Direction
- Detects language from i18next
- Applies appropriate text direction
- Adjusts spacing and positioning

### RTL Optimizations
- **Header layout**: Reversed flex direction
- **Close button**: Proper positioning
- **Text alignment**: Right-aligned for Arabic
- **Spacing**: RTL-aware margins and padding

## Testing

### Component Testing

```tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { Popup } from '@/shared/components';

test('opens and closes popup', () => {
  const onClose = jest.fn();
  
  render(
    <Popup isOpen={true} onClose={onClose} title="Test Popup">
      Test content
    </Popup>
  );
  
  expect(screen.getByText('Test Popup')).toBeInTheDocument();
  
  fireEvent.click(screen.getByLabelText('Close popup'));
  expect(onClose).toHaveBeenCalled();
});
```

### Hook Testing

```tsx
import { renderHook, act } from '@testing-library/react';
import { usePopup } from '@/shared/hooks/usePopup';

test('manages popup state', () => {
  const { result } = renderHook(() => usePopup());
  
  expect(result.current.isOpen).toBe(false);
  
  act(() => {
    result.current.open();
  });
  
  expect(result.current.isOpen).toBe(true);
});
```

## Best Practices

### 1. Size Selection
- Use `sm` for simple confirmations
- Use `md` for standard forms
- Use `lg`+ for complex interfaces

### 2. Animation Choice
- Use `scale` for most cases
- Use `fade` for subtle notifications
- Use `none` for performance-critical scenarios

### 3. Content Structure
- Keep content focused and relevant
- Use clear, descriptive titles
- Provide context through descriptions

### 4. Accessibility
- Always provide meaningful titles
- Use descriptions for complex popups
- Test with keyboard navigation
- Verify screen reader announcements

### 5. Performance
- Lazy load popup content when possible
- Use React.memo for heavy content
- Implement proper cleanup in useEffect hooks

## Examples

See the UserManagementView component for real-world usage examples of the popup system in action.
